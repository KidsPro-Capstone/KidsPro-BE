﻿using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Course.Quiz;

public record CreateQuizDto
{

    [StringLength(250, ErrorMessage = "Title can not exceed 250 characters")]
    public string Title { get; init; } = null!;

    [StringLength(750, ErrorMessage = "description can not exceed 750 characters.")]
    public string? Description { get; init; }

    public int? Duration { get; init; }

    public int? NumberOfQuestion { get; init; }

    public bool? IsOrderRandom { get; init; }

    public int? NumberOfAttempt { get; init; }

    public ICollection<CreateQuestionDto> Questions { get; init; } = null!;
}