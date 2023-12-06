﻿namespace Application.Dtos.Response;

public class PagingResponse<T>
{
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public ICollection<T> Results { get; set; } = new List<T>();
}