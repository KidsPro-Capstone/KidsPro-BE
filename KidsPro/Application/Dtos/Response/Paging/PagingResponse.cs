﻿namespace Application.Dtos.Response.Paging;

public class PagingResponse<T> : IPagingResponse<T>
{
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public IEnumerable<T> Results { get; set; } = new List<T>();
}