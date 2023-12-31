﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Generic;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public class Order:BaseEntity
{

    [StringLength(250)] public string PaymentMethod { get; set; } = null!;

    [Range(0, float.MaxValue)] [Precision(11,2)] public decimal TotalPrice { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    [Precision(2)]
    public DateTime Date { get; } = DateTime.UtcNow;

    [StringLength(750)] public string? Note { get; set; }

    [Column(TypeName = "tinyint")] public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [Required] public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public int? CurriculumId { get; set; }

    public virtual Curriculum? Curriculum { get; set; }
}