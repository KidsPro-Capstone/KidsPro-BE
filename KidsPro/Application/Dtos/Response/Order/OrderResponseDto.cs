﻿using Domain.Enums;

namespace Application.Dtos.Response.Order;

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public string? OrderCode { get; set; }
    public string? PictureUrl { get; set; }
    public string? CourseName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? OrderStatus { get; set; }
}