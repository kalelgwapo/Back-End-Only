using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/leave-applications")]
public class LeaveApplicationsController : ControllerBase
{

    private readonly ILeaveApplicationService _leaveApplicationService;
    

    public LeaveApplicationsController(ILeaveApplicationService leaveApplicationService)
    {
        _leaveApplicationService = leaveApplicationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveApplicationResponse>>> GetAllLeaveApplications(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _leaveApplicationService.GetAllLeaveApplicationsAsync(page, pageSize, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveApplicationResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid leave application ID." });
        }
        var application = await _leaveApplicationService.GetByApplicationIdAsync(id, cancellationToken);
        return application is null ? NotFound() : Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveApplicationResponse>> Post(
        LeaveApplicationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return BadRequest(new { message = "Request body cannot be null." });
            }

            var application = await _leaveApplicationService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = application.Id }, application);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
