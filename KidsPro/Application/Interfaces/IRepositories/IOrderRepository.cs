﻿using Application.Interfaces.IRepositories.Generic;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IOrderRepository:IBaseRepository<Order>
    {
        Task<(Order?, string?)> GetByOrderCode(Func<int, string> GenerateOrderCode, bool decision);
    }
}