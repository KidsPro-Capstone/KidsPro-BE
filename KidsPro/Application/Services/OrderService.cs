﻿using Application.Dtos.Request.Order;
using Application.Dtos.Request.Progress;
using Application.Dtos.Response.Order;
using Application.ErrorHandlers;
using Application.Interfaces.IServices;
using Application.Mappers;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        readonly IUnitOfWork _unitOfWork;
        private IAccountService _account;
        private INotificationService _notify;
        private IPaymentService _payment;
        private ICourseService courseService;

        public OrderService(IUnitOfWork unitOfWork, IAccountService account, INotificationService notify,
            IPaymentService payment, ICourseService courseService)
        {
            _unitOfWork = unitOfWork;
            _account = account;
            _notify = notify;
            _payment = payment;
            this.courseService = courseService;
        }


        public async Task<int> CreateOrderAsync(OrderRequest dto)
        {
            var voucher = await _unitOfWork.VoucherRepository.GetVoucherPaymentAsync(dto.VoucherId);
            //Check lấy order code, nếu đã tồn tại phải tạo ordercode mới
            string? getOrderCode;
            Course? course;
            do
            {
                //Create and check exist order code
                var checkOrderCode =
                    await _unitOfWork.OrderRepository.GetByOrderCode(StringUtils.GenerateRandomNumber);
                getOrderCode = checkOrderCode.Item1 != null ? null : checkOrderCode.Item2;
                course = await _unitOfWork.CourseRepository.CheckCourseExist(dto.CourseId);
                if (course == null) throw new BadRequestException($"CourseId {dto.CourseId} doesn't exist");
            } while (getOrderCode == null);

            var account = await _account.GetCurrentAccountInformationAsync();

            //Create Order
            var order = new Order()
            {
                ParentId = account.IdSubRole,
                VoucherId = voucher != null ? dto.VoucherId : null,
                PaymentType = (PaymentType)dto.PaymentType,
                Quantity = dto.Quantity,
                TotalPrice = (course.Price * dto.Quantity) - (voucher?.DiscountAmount ?? 0),
                Date = DateTime.UtcNow,
                Status = OrderStatus.Process,
                OrderCode = getOrderCode,
                Note = "course: " + course.Name
            };

            //Create OrderDetail
            var orderDetail = new OrderDetail()
            {
                Price = course.Price,
                CourseId = dto.CourseId,
                ClassId = dto.ClassId,
                Quantity = dto.Quantity,
                Order = order
            };

            //Create OrderDetail Student
            foreach (var x in dto.StudentId)
            {
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(x);
                if (student != null)
                {
                    // Kiểm tra xem danh sách OrderDetails đã tồn tại hay chưa
                    if (student.OrderDetails.Count == 0)
                        student.OrderDetails = new List<OrderDetail>();

                    // Thêm _orderDetail vào danh sách hiện có
                    student.OrderDetails.Add(orderDetail);
                }
            }

            //Add data to database
            await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            //update lai status voucher
            if (voucher != null)
            {
                voucher.Status = VoucherStatus.Used;
                _unitOfWork.VoucherRepository.Update(voucher);
            }

            var result = await _unitOfWork.SaveChangeAsync();
            if (result <= 0)
                throw new NotImplementException("Create Order Failed");
            //Mapper
            return order.Id;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus currentStatus, OrderStatus toStatus,
            string? reason = "")
        {
            var order = await GetOrderByStatusAsync(orderId, currentStatus);
            if (order != null)
            {
                switch (currentStatus)
                {
                    case OrderStatus.Process:
                        order.Status = toStatus;
                        break;
                    case OrderStatus.Pending:
                        if (toStatus == OrderStatus.RequestRefund)
                            order.Note = reason;
                        order.Status = toStatus;
                        break;
                    case OrderStatus.RequestRefund:
                        order.Status = toStatus;
                        break;
                }

                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangeAsync();
                return;
            }

            throw new NotImplementException($"Update orderID:{orderId} " +
                                            $"to {currentStatus} status from {toStatus} status failed");
        }

        public async Task<PagingOrderResponse> GetListOrderAsync(OrderStatus status, int? pageSize, int? pageNumber)
        {
            //set default page size
            if (!pageSize.HasValue || !pageNumber.HasValue)
            {
                pageSize = 10;
                pageNumber = 1;
            }

            var account = await _account.GetCurrentAccountInformationAsync();
            var orders = await _unitOfWork.OrderRepository.GetListOrderAsync(status, account.IdSubRole, account.Role,
                pageSize.Value, pageNumber.Value);

            return OrderMapper.OrdersToPagingOrderResponse(orders);
        }

        public async Task<List<OrderResponse>> MobileGetListOrderAsync(OrderStatus status)
        {
            var account = await _account.GetCurrentAccountInformationAsync();
            var orders =
                await _unitOfWork.OrderRepository.MobileGetListOrderAsync(status, account.IdSubRole, account.Role);
            return OrderMapper.MobileShowOrder(orders!);
        }

        public async Task<OrderDetailResponse> GetOrderDetail(int orderId)
        {
            var account = await _account.GetCurrentAccountInformationAsync();
            var order = await _unitOfWork.OrderRepository.GetOrderDetail(account.IdSubRole, orderId, account.Role)
                        ?? throw new UnauthorizedException("OrderId doesn't exist");
            return OrderMapper.ShowOrderDetail(order);
        }

        public async Task ParentCanCelOrderAsync(OrderCancelRequest dto)
        {
            await UpdateOrderStatusAsync(dto.OrderId, OrderStatus.Pending, OrderStatus.RequestRefund, dto.Reason);
        }

        public async Task HandleRefundRequest(OrderRefundRequest dto, ModerationStatus status)
        {
            string title = "";
            string content = "";
            switch (status)
            {
                case ModerationStatus.Approve:
                    var responseMomo = await _payment.RequestMomoRefundAsync(dto.OrderId);
                    // Nếu result code != 0 => Refund failed
                    if (responseMomo.resultCode > 0)
                        throw new BadRequestException("Momo refuse request refund, Please debug");
                    // Update order status
                    await UpdateOrderStatusAsync(dto.OrderId, OrderStatus.RequestRefund, OrderStatus.Refunded);
                    // Update transaction status
                    await _payment.UpdateTransStatusToRefunded(responseMomo.orderId);
                    // Send a notice of acceptance of order cancellation to parent
                    title = "The result of processing order cancellation request";
                    content = "Order cancellation request accepted, " +
                              "KidsPro has successfully refunded money to Momo e-Wallet ";
                    break;
                case ModerationStatus.Refuse:
                    await UpdateOrderStatusAsync(dto.OrderId, OrderStatus.RequestRefund, OrderStatus.Pending);
                    // Send a notice of refusal of order cancellation to parent
                    title = "The result of processing order cancellation request";
                    content = "Order cancellation request refused because  " + dto.ReasonRefuse;
                    break;
            }

            await _notify.SendNotifyToAccountAsync(dto.ParentId, title, content);
        }

        public async Task<Order?> GetOrderByStatusAsync(int orderId, OrderStatus status)
        {
            //var account = await _account.GetCurrentAccountInformationAsync();

            var order = await _unitOfWork.OrderRepository.GetOrderByStatusAsync(orderId, status);
            return order ??
                   throw new NotFoundException($"OrderId {orderId} not exist process status");
        }

        public async Task<OrderResponse?> SearchOrderByCodeAsync(string code)
        {
            var order = await _unitOfWork.OrderRepository.SearchByOrderCode(code)
                        ?? throw new NotFoundException("OrderCode " + code + " not found");
            return OrderMapper.OrderToOrderResponse(order);
        }

        public async Task ConfirmOrderAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId)
                        ?? throw new BadRequestException("OrderId" + orderId + " not found");
            var progress = new StudentProgressRequest()
            {
                CourseId = order.OrderDetails!.FirstOrDefault().CourseId,
                SectionId = order.OrderDetails!.FirstOrDefault().Course.Sections.FirstOrDefault().Id
            };

            await courseService.StartStudyCourseAsync(progress,
                order.OrderDetails!.FirstOrDefault().Students.Select(x => x.Id).ToList());

            await UpdateOrderStatusAsync(orderId, OrderStatus.Pending, OrderStatus.Success);
        }
    }
}