﻿using Application.Interfaces.IRepositories.Generic;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Response.Order;
using Application.Dtos.Response.Paging;
using Domain.Enums;

namespace Application.Interfaces.IRepositories
{
    public interface IOrderRepository:IBaseRepository<Order>
    {
        Task<(Order?, string?)> GetByOrderCode(Func<int, string> generateOrderCode, bool decision);
        Task<Order?> GetOrderByStatusAsync( int orderId,OrderStatus status);

        Task<PagingResponse<Order>> GetListOrderAsync(OrderStatus status, int parentId, string role, int pageSize,
            int pageNumber);
        Task<List<Order>?> MobileGetListOrderAsync(OrderStatus status, int parentId, string role);
        Task<Order?> GetOrderDetail(int parentId, int orderId, string role);
    }
}
