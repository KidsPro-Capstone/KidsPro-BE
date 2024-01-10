using Application.Dtos.Request.Course;
using Application.Dtos.Response.Course;
using Application.Dtos.Response.Paging;
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
    /// Admin, staff, teacher lấy và filter danh sách course có trên hệ thống cho mục đích quản lý. Tất cả param đều không bắt buộc.
    /// Các param cho mục đích sort có thể có value "asc", "desc".
    /// </summary>
    /// <param name="name"></param>
    /// <param name="status"></param>
    /// <param name="sortName"></param>
    /// <param name="sortCreatedDate"></param>
    /// <param name="sortModifiedDate"></param>
    /// <param name="isOfCurrentUser"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet("manage")]
    [Authorize(Roles = "Admin,Teacher,Staff")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResponse<CommonManageCourseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<PagingResponse<CommonManageCourseDto>>> GetManageCourseAsync(
        [FromQuery] string? name,
        [FromQuery] string? status,
        [FromQuery] string? sortName,
        [FromQuery] string? sortCreatedDate,
        [FromQuery] string? sortModifiedDate,
        [FromQuery] int? page,
        [FromQuery] int? size,
        [FromQuery] bool? isOfCurrentUser
    )
    {
        var result = await _courseService.GetManageCourseAsync(
            name,
            status,
            sortName,
            sortCreatedDate,
            sortModifiedDate,
            page,
            size,
            isOfCurrentUser ?? false
        );

        return Ok(result);
    }

    /// <summary>
    /// Người dùng thông thường có thể lấy danh sách các khóa học hiện có trên hệ thống (status = Active).
    /// Api không yêu cầu đăng nhập, các param đều không bắt buộc. Mặc định page = 1, size = 10 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sortName"></param>
    /// <param name="sortPostedDate"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagingResponse<CommonCourseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<PagingResponse<CommonCourseDto>>> GetCommonCourseAsync(
        [FromQuery] string? name,
        [FromQuery] string? sortName,
        [FromQuery] string? sortPostedDate,
        [FromQuery] int? page,
        [FromQuery] int? size
    )
    {
        var result = await _courseService.GetCoursesAsync(
            name, sortName, sortPostedDate, page, size);
        return Ok(result);
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
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<CourseDto>> UpdateAsync([FromRoute] int id, [FromBody] UpdateCourseDto request)
    {
        return Ok(await _courseService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Upload ảnh bìa cho khóa học 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/picture")]
    [Authorize(Roles = "Admin,Teacher,Staff")]
    [Consumes("image/jpeg", "image/png", "multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseDto))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult<CourseDto>> UpdatePictureAsync([FromRoute] int id,
        [FromForm(Name = "file")] IFormFile file)
    {
        return Ok(await _courseService.UpdatePictureAsync(id, file));
    }

    /// <summary>
    ///  Delete unused course. Only admin or owner can delete course by Id and can only delete draft course
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Teacher,Staff")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorDetail))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorDetail))]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        await _courseService.DeleteAsync(id);
        return NoContent();
    }
}