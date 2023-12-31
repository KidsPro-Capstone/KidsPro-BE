﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index(nameof(StudentQuizId), nameof(QuestionId), IsUnique = true)]
public class StudentAnswer:BaseEntity
{

    [Range(0, int.MaxValue)] [Precision(5,2)] public decimal Score { get; set; }

    public int StudentQuizId { get; set; }

    public virtual StudentQuiz StudentQuiz { get; set; } = null!;

    public int QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<StudentAnswerOption> StudentAnswerOptions { get; set; } = new List<StudentAnswerOption>();
}