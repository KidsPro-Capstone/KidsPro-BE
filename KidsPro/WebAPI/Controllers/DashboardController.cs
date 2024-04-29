﻿using Application.Configurations;
using Application.Dtos.Response;
using Application.Interfaces.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private IDashboardService _dashboard;
    private IQuizService _quiz;

    public DashboardController(IDashboardService dashboard, IQuizService quiz)
    {
        _dashboard = dashboard;
        _quiz = quiz;
    }

    /// <summary>
    ///  Admin get dashboard
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.AdminRole}")]
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> AdminGetDashboardAsync()
    {
        var result = await _dashboard.GetDashboardAsync();
        return result;
    }
}