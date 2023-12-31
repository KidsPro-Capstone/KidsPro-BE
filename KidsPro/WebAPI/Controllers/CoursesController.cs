﻿using Application.Dtos.Request.Course;
using Application.Dtos.Response.Course;
using Application.ErrorHandlers;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/v1/courses")]
[ApiController]
public class CoursesController : Controller
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Admin hoặc teacher có thể tạo khóa học trên hệ thống. Khóa học được tạo sẽ có status draft, cần phải 
    /// tạo yêu cầu duyệt 
    /// để đưa khóa học lên trên website.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<CourseDto>> CreateAsync([FromBody] CreateCourseDto request)
    {
        var result = await _courseService.CreateAsync(request);
        return Created(nameof(CreateAsync), result);
    }

    /// <summary>
    /// Admin hoặc người tạo khóa học có thể thay đổi thông tin cơ bản của khóa học.
    /// Chỉ có thể thay đổi thông tin khi khóa học có status draft
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<CourseDto>> UpdateAsync([FromRoute] int id, [FromBody] UpdateCourseDto request)
    {
        return Ok(await _courseService.UpdateAsync(id, request));
    }
}