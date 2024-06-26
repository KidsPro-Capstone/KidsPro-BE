﻿using Application.Dtos.Response.Account.Student;
using Application.Dtos.Response.Order;
using Application.Utils;
using Domain.Entities;

namespace Application.Mappers
{
    public class OrderMapper
    {
        public static  List<OrderResponse> MobileShowOrder(List<Order> orders)
        {
            var list = new List<OrderResponse>();
            foreach (var x in orders)
            {
                var dto = new OrderResponse()
                {
                    OrderId = x.Id,
                    OrderCode = x.OrderCode,
                    CourseName = x.OrderDetails!.FirstOrDefault()?.Course.Name,
                    PictureUrl = x.OrderDetails!.FirstOrDefault()?.Course.PictureUrl,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice,
                    OrderStatus = x.Status.ToString(),
                    ParentName = x.Parent!.Account.FullName,
                    PaymentType = x.PaymentType.ToString(),
                    Note = x.Note
                };
                list.Add(dto);
            }

            return list;
        }

        public static (int, List<OrderResponse>) ShowOrder(List<Order> orders)
        {
            int totalOrder = orders.Count;

            var list = new List<OrderResponse>();
            foreach (var x in orders)
            {
                var dto = new OrderResponse()
                {
                    OrderId = x.Id,
                    OrderCode = x.OrderCode,
                    CourseName = x.OrderDetails!.FirstOrDefault()?.Course.Name,
                    PictureUrl = x.OrderDetails!.FirstOrDefault()?.Course.PictureUrl,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice,
                    OrderStatus = x.Status.ToString(),
                    ParentName = x.Parent!.Account.FullName,
                    PaymentType = x.PaymentType.ToString(),
                    Note = x.Note
                };
                list.Add(dto);
            }

            return (totalOrder, list);
        }

        public static OrderDetailResponse ShowOrderDetail(Order order)
        {
            var x = new OrderDetailResponse()
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                PictureUrl = order.OrderDetails!.FirstOrDefault()!.Course.PictureUrl,
                CourseName = order.OrderDetails!.FirstOrDefault()!.Course.Name,
                Price = order.OrderDetails!.FirstOrDefault()!.Course.Price,
                QuantityPurchased = order.Quantity,
                OrderDate = DateUtils.FormatDateTimeToDatetimeV3(order.Date),
                OrderCode = order.OrderCode,
                PaymentType = order.PaymentType.ToString(),
                TransactionCode = order.Transaction?.TransactionCode,
                ParentEmail = order.Parent!.Account.Email,
                ParentZalo = order.Parent!.PhoneNumber,
                TotalPrice = order.TotalPrice,
                Discount = order.Voucher?.DiscountAmount,
                NumberChildren = order.OrderDetails!.FirstOrDefault()!.Students.Count,
                ParentName = order.Parent!.Account.FullName,
                ParentId = order.Parent!.Account.Id,
                ClassId = order.OrderDetails!.FirstOrDefault()!.Class?.Id,
                ClassCode = order.OrderDetails!.FirstOrDefault()!.Class?.Code
            };
            foreach (var dto in order.OrderDetails!.FirstOrDefault()!.Students)
            {
                var stu = new StudentOrderDetail()
                {
                    StudentId = dto.Id,
                    StudentName = dto.Account.FullName,
                    DateOfBirth = DateUtils.FormatDateTimeToDateV3(dto.Account.DateOfBirth),
                    UserName = dto.UserName
                };
                x.Students?.Add(stu);
            }

            return x;
        }
    }
}