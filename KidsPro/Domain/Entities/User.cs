﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Generic;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public class User : BaseEntity
{
    [MaxLength(100)] public string FullName { get; set; } = string.Empty;
    [MaxLength(11)] public string PhoneNumber { get; set; } = string.Empty;
    [MaxLength(150)] public string PasswordHash { get; set; } = string.Empty;
    [MaxLength(250)] public string? PictureUrl { get; set; }
    [Column(TypeName = "tinyint")] public Gender? Gender { get; set; }
    [Column(TypeName = "tinyint")] public UserStatus Status { get; set; } = UserStatus.Active;
    public bool IsDelete { get; set; } = false;

    [DataType(DataType.DateTime)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
    [Precision(2)]
    public DateTime CreatedDate { get; } = DateTime.UtcNow;

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public int? TeacherId { get; set; }

    [ForeignKey(nameof(TeacherId))] public virtual Teacher? Teacher { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    // add relation for transaction table
    public virtual ICollection<Transaction> CreatedTransactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> ProcessedTransactions { get; set; } = new List<Transaction>();

    //relationship for curriculum table
    public virtual ICollection<Curriculum> CreatedCurriculums { get; set; } = new List<Curriculum>();

    //relationship for course table
    public virtual ICollection<Course> CreatedCourses { get; set; } = new List<Course>();

    public virtual Cart? Cart { get; set; }
}