﻿using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Course;

public record UpdateCourseDto
{
    [Required]
    public byte[] Version { get; set; } = null!;
    
    [StringLength(250, ErrorMessage = "Class name can not exceed 250 character")]
    [Required(ErrorMessage = "Class name is required.")]
    public string? Name { get; set; } = null!;

    [StringLength(3000, ErrorMessage = "Description can not exceed 3000 character")]
    public string? Description { get; set; }

    [StringLength(250, ErrorMessage = "Prerequisite can not exceed 250 character")]
    public string? Prerequisite { get; set; }

    [StringLength(150, ErrorMessage = "Language can not exceed 150 character")]
    public string? Language { get; set; }

    [StringLength(250, ErrorMessage = "Graduate condition can not exceed 250 character")]
    public string? GraduateCondition { get; set; }

    [Range(3, 30)] public int? FromAge { get; set; }

    [Range(3, 30)] public int? ToAge { get; set; }

    public bool? IsFree { get; set; }
}