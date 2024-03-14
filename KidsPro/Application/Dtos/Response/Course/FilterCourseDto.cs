﻿namespace Application.Dtos.Response.Course;

public class FilterCourseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? PictureUrl { get; set; }
    
    public bool IsFree { get; set; }
    
    public string Status { get; set; } = string.Empty;
}