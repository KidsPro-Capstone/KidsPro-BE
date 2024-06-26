﻿using Domain.Enums;

namespace Application.Dtos.Response;

public class ClassesResponse
{
    public ClassStatus ClassStatus { get; set; }
    public int CourseId { get; set; }
    public string? CourseImage { get; set; }
    public string? CourseName { get; set; }
    public int ClassId { get; set; }
    public string? ClassCode { get; set; }
    public string? DayStart { get; set; }
    public string? DayOfWeekStart { get; set; }
    public string? DayEnd { get; set; }
    public string? DayOfWeekEnd { get; set; }
    public List<DayStatus>? Days { get; set; } = new List<DayStatus>();
    public string? SlotStart { get; set; }
    public string? SlotEnd { get; set; }
    public string? Teacher { get; set; }
    public float CourseProgress { get; set; }
}