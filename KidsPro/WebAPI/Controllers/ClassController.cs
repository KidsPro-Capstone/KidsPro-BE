﻿using Application.Configurations;
using Application.Dtos.Request.Class;
using Application.Dtos.Response;
using Application.Dtos.Response.Account.Student;
using Application.Dtos.Response.StudentSchedule;
using Application.ErrorHandlers;
using Application.Interfaces.IServices;
using Domain.Enums;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class ClassController : ControllerBase
{
    private IClassService _class;
    private IAuthenticationService _authentication;

    public ClassController(IClassService @class, IAuthenticationService authentication)
    {
        _class = @class;
        _authentication = authentication;
    }

    #region Class

    /// <summary>
    /// Create Class
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<ClassCreateResponse>> CreateClassAsync(ClassCreateRequest dto)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.CreateClassAsync(dto);
        return Ok(result);
    }
    
    /// <summary>
    /// Get class detail 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpGet("detail/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<ClassResponse>> GetClassDetailAsync(int id)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.GetClassByIdAsync(id);
        return Ok(result);
    }

    #endregion

    #region Schedule

    /// <summary>
    /// Create Schedules
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpPost("schedules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<ScheduleCreateResponse>> CreateScheduleAsync(ScheduleCreateRequest dto)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.CreateScheduleAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Get schedule of this class by classId
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpGet("schedules/{classId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<ClassResponse>> GetScheduleByClassIdAsync(int classId)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.GetScheduleByClassIdAsync(classId);
        return Ok(result);
    }

    /// <summary>
    /// Update schedule
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpPut("schedules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<ClassResponse>> UpdateScheduleAsync(ScheduleUpdateRequest dto)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        await _class.UpdateScheduleAsync(dto);
        return Ok(new
        {
            Message = "Update Schedules Successfully"
        });
    }
    #endregion

    #region Teacher
    /// <summary>
    /// Get all teacher with activate status in the system, including their schedule
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpGet("teachers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<List<TeacherScheduleResponse>>> GetTeacherAsync()
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.GetTeacherToClassAsync();
        return Ok(result);
    }
    
    /// <summary>
    /// Add Teacher to class
    /// </summary>
    /// <param name="classId"></param>
    /// <param name="teacherId"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpPost("teacher/add-to-class")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<string>> AddTeacherAsync(int classId, int teacherId)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.AddTeacherToClassAsync(teacherId, classId);
        return Ok(new
        {
            Message = Ok(),
            TeacherName = result
        });
    }
    #endregion

    #region Student
    /// <summary>
    /// Search students name, BE checked and dropped students with overlap schedules before return the list
    /// </summary>
    /// <param name="input"></param>
    /// <param name="classId"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpGet("students/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<StudentClassResponse>> SearchStudentAsync(string input, int classId)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.SearchStudentScheduleAsync(input, classId);
        return Ok(result);
    }

    /// <summary>
    /// Add students to class or remove student from class
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole},{Constant.AdminRole}")]
    [HttpPost("students/add-or-remove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<StudentClassResponse>> AddStudentsToClassAsync(StudentsAddRequest dto,
        ClassStudentType type)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        var result = await _class.UpdateStudentsToClassAsync(dto, type);
        return Ok(result);
    }
    #endregion

    

    

  

   

    
}