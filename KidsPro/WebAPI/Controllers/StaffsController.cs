﻿using Application.Configurations;
using Application.Dtos.Request.Account.Student;
using Application.Dtos.Request.Email;
using Application.ErrorHandlers;
using Application.Interfaces.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/v1/staffs")]
public class StaffsController : ControllerBase
{
    private IStaffService _staff;
    private INotificationService _notify;
    private IAuthenticationService _authentication;
    
    public StaffsController(IStaffService staff, INotificationService notify, IAuthenticationService authentication)
    {
        _staff = staff;
        _notify = notify;
        _authentication = authentication;
    }

    /// <summary>
    /// Staff Create Student Account
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole}")]
    [HttpPost("student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<EmailContentRequest>> CreateAccountAsync(StudentCreateAccountRequest dto)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();
        
        var result=await _staff.CreateAccountStudentAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Send request to parent create email for student
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="studentName"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole}")]
    [HttpPost("parent/request-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> RequestEmailAsync(int parentId, string studentName)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();
        
        var title = "Request Create Email For Student";
        var content = "Please create an email for student " + studentName +
                      ", An email used to access to the website, study online and login to the game";
        await _notify.SendNotifyToAccountAsync(parentId, title, content);
        return Ok("Send request to parent successfully");
    }
    
    /// <summary>
    /// After create student account successful, staff sent an account to parent's email
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole}")]
    [HttpPost("parent/send-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> SendEmailAsync(EmailContentRequest request)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();

        await _staff.SendEmailParentAsync(request);

        return Ok();
    }
    
    /// <summary>
    /// View reason for cancellation
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [Authorize(Roles = $"{Constant.StaffRole}")]
    [HttpGet("order/view-reason/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    public async Task<IActionResult> ViewReasonAsync(int orderId)
    {
        //Check if the account is activated or not or inactive
        _authentication.CheckAccountStatus();
        
        var result = await _staff.ViewReasonOrderCancel(orderId);
        return Ok(new
        {
            Reason=result
        });
    }
}