﻿using Application.Dtos.Request.Order.Momo;
using Application.Dtos.Response.Order;
using Application.Dtos.Response.Order.Momo;
using Application.ErrorHandlers;
using Application.Interfaces.IServices;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Services;

public class PaymentService : IPaymentService
{
    readonly IUnitOfWork _unitOfWork;
    readonly IOrderService _orderService;
    private IAccountService _accountService;

    public PaymentService(IUnitOfWork unitOfWork, IOrderService orderService, IAccountService accountService)
    {
        _unitOfWork = unitOfWork;
        _orderService = orderService;
        _accountService = accountService;
    }

    public async Task<Order?> GetOrderStatusPaymentAsync(int orderId)
    {
        var account = await _accountService.GetCurrentAccountInformationAsync();
        
        var order = await _unitOfWork.OrderRepository.GetOrderByStatusAsync(account.IdSubRole, orderId,
            OrderStatus.Payment);
        if (order != null)
            return order;
        throw new NotFoundException($"OrderId {orderId} of ParentId {account.IdSubRole} doesn't payment status");
    }

    public string MakeSignatureMomoPayment(string accessKey, string secretKey, MomoPaymentRequest momo)
    {
        var rawHash = "accessKey=" + accessKey +
                      "&amount=" + momo.amount + "&extraData=" + momo.extraData +
                      "&ipnUrl=" + momo.ipnUrl + "&orderId=" + momo.orderId +
                      "&orderInfo=" + momo.orderInfo + "&partnerCode=" + momo.partnerCode +
                      "&redirectUrl=" + momo.redirectUrl + "&requestId=" + momo.requestId + "&requestType=" +
                      momo.requestType;
        return momo.signature = HashingUtils.HmacSha256(rawHash, secretKey);
    }

    public (string?, string?) GetLinkGatewayMomo(string paymentUrl, MomoPaymentRequest momoRequest)
    {
        using HttpClient client = new HttpClient();
        var requestData = JsonConvert.SerializeObject(momoRequest, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        });
        var requestContent = new StringContent(requestData, Encoding.UTF8, "application/json");

        var createPaymentLink = client.PostAsync(paymentUrl, requestContent).Result;
        if (createPaymentLink.IsSuccessStatusCode)
        {
            var responseContent = createPaymentLink.Content.ReadAsStringAsync().Result;
            var responeseData = JsonConvert.DeserializeObject<MomoPaymentResponse>(responseContent);
            // return QRcode
            if (responeseData?.resultCode == "0")
                return (responeseData.payUrl, responeseData.qrCodeUrl);
            throw new NotImplementException($"Error Momo: {responeseData?.message}");
        }
        else
            throw new NotImplementException($"Error Momo: {createPaymentLink.ReasonPhrase}");
    }

    private  int GetIdMomoResponse(string id)
    {
        Regex regex = new Regex("-(\\d+)");
        var macth = regex.Match(id);
        if (macth.Success) return Int32.Parse(macth.Groups[1].Value);
        return 0;
    }
    public async Task CreateTransactionAsync(MomoResultRequest dto)
    {
        var orderId = GetIdMomoResponse(dto.orderId);
        var parentId = GetIdMomoResponse(dto.requestId);

        await _orderService.UpdateOrderStatusAsync(orderId, parentId, OrderStatus.Payment, OrderStatus.Pending);
        var transaction = new Transaction()
        {
            OrderId = orderId,
            CreatedDate = DateTime.UtcNow,
            TransactionCode = dto.transId,
            Amount = dto.amount,
            Status = TransactionStatus.Success
        };
        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        var result = await _unitOfWork.SaveChangeAsync();
        if (result < 0) throw new NotImplementException("Add transaction failed");
    }
}