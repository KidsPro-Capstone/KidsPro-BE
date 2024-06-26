﻿using System.Text.Json.Serialization;

namespace Application.Dtos.Request.Course.Quiz;

public class QuizSubmitRequest
{
    [JsonIgnore]
    public int StudentId { get; set; }
    public int QuizId { get; set; }
    public List<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
    [JsonIgnore] public int Score { get; set; } = 0;
    public int Duration { get; set; }
    public int QuizMinutes { get; set; }
}