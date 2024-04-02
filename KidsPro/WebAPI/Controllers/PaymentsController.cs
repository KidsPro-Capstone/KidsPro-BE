﻿using Application.Dtos.Request.Order.Momo;
using Application.Dtos.Request.Order.ZaloPay;
using Application.Dtos.Response.Order;
using Application.Interfaces.IServices;
using Application.Utils;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Gateway.IConfig;

namespace WebAPI.Controllers
{
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        IPaymentService _payment;
        IMomoConfig _momoConfig;
        IAuthenticationService _authentication;
        IZaloPayConfig _zaloPayConfig;

        public PaymentsController(IPaymentService payment, IMomoConfig momoConfig,
            IAuthenticationService authentication, IZaloPayConfig zaloPayConfig)
        {
            _payment = payment;
            _momoConfig = momoConfig;
            _authentication = authentication;
            _zaloPayConfig = zaloPayConfig;
        }

        #region Momo

        /// <summary>
        /// Tạo link url payment Momo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("momo/{id}")]
        public async Task<ActionResult> CreatePaymentMomoAsync(int id)
        {
            //Check if the account is activated or not or inactive
            _authentication.CheckAccountStatus();

            var momoRequest = new MomoPaymentRequest();
            //Get order có parent id và order id vs status payment
            var order = await _payment.GetOrderStatusPaymentAsync(id);
            if (order == null) return BadRequest($"OrderID:{id} doesn't not exist");

            // Lấy thông tin cho payment
            momoRequest.requestId = StringUtils.GenerateRandomNumber(4) + "-" + order.ParentId;
            momoRequest.orderId = StringUtils.GenerateRandomNumber(4) + "-" + order.Id;
            momoRequest.amount = (long)order.TotalPrice;
            momoRequest.redirectUrl = _momoConfig.ReturnUrl;
            momoRequest.ipnUrl = _momoConfig.IpnUrl;
            momoRequest.partnerCode = _momoConfig.PartnerCode;
            momoRequest.orderInfo = " 'KidsPro Service' - You are payment for " + order.Note;
            momoRequest.signature = _payment.MakeSignatureMomoPayment
                (_momoConfig.AccessKey, _momoConfig.SecretKey, momoRequest);

            // lấy link QR momo
            var result = _payment.GetLinkGatewayMomo(_momoConfig.PaymentUrl, momoRequest);
            return Ok(new
            {
                payUrl = result.Item1,
                qrCode = result.Item2,
            });
        }

        /// <summary>
        /// Staff call this api, momo refund
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("momo/refund/{id}")]
        public async Task<ActionResult> RefundMomoAsync(int id)
        {
            //Check if the account is activated or not or inactive
            _authentication.CheckAccountStatus();

            var momoRequest = new MomoRefundRequest();
            //Get Transaction by orderId
            var transaction = await _payment.GetTransactionByOrderIdAsync(id);

            // Lấy thông tin cho payment
            momoRequest.requestId = StringUtils.GenerateRandomNumber(4) + "-" + transaction.Order!.ParentId;
            momoRequest.orderId = StringUtils.GenerateRandomNumber(4) + "-" + transaction.OrderId;
            momoRequest.amount = 1000;
            momoRequest.partnerCode = _momoConfig.PartnerCode;
            momoRequest.transId = long.Parse(transaction.TransactionCode!);
            momoRequest.signature = _payment.MakeSignatureMomoRefund
                (_momoConfig.AccessKey, _momoConfig.SecretKey, momoRequest);

            // Request Momo Refund
            var result = _payment.RequestMomoRefund(_momoConfig.RefundUrl, momoRequest);
            return Ok(result);
        }

        /// <summary>
        /// Không dùng APi này, sau khi payment success Momo sẽ tự động gọi api return
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("momo-return")]
        public async Task<IActionResult> MomoReturnAsync([FromQuery] MomoResultRequest dto)
        {
            var orderId = await _payment.CreateTransactionAsync(dto);
            return Redirect($"https://capstone1-phi.vercel.app/payment-success/{orderId}");
        }

        #endregion

        #region ZaloPay

        /// <summary>
        /// Tạo link url payment ZaloPay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("zalopay/{id}")]
        public async Task<ActionResult> CreatePaymentZaloPayAsync(int id)
        {
            //Check if the account is activated or not or inactive
            _authentication.CheckAccountStatus();

            var zaloRequest = new ZaloPaymentRequest();
            //Get order có parent id và order id vs status payment
            var order = await _payment.GetOrderStatusPaymentAsync(id);
            if (order == null) return BadRequest($"OrderID:{id} doesn't not exist");

            // Lấy thông tin cho payment
            zaloRequest.AppUser = order.Parent!.Account.FullName;
            zaloRequest.AppTransId = DateUtils.FormatDateTimeToDateV4(order.Date) + "_" + order.Id;
            zaloRequest.Amount = (long)order.TotalPrice;
            zaloRequest.AppTime = TimeUtils.GetOrderTimeSpan(order.Date);
            zaloRequest.AppId = Int32.Parse(_zaloPayConfig.AppId);
            zaloRequest.Description = "ZaloPayDemo - Thanh toán cho đơn hàng #220817_1660717311101";
            zaloRequest.BankCode = "zalopayapp";
            // zaloRequest.embed_data = "{\"redirecturl\": \"https://docs.zalopay.vn/result\"}";
            zaloRequest.Mac = _payment.MakeSignatureZaloPayment(_zaloPayConfig.Key1, zaloRequest);

            // lấy link QR momo
            var result = _payment.GetLinkGatewayZaloPay(_zaloPayConfig.PaymentUrl, zaloRequest);
            return Ok(new
            {
                payUrl = result,
            });
        }

        #endregion
    }
}