using Api.Models;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Lookup")]
public class LookupsController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly ILeaveApplicationService _leaveApplicationService;

    public LookupsController(
        IUserService userService,
        ILeaveApplicationService leaveApplicationService)
    {
        _userService = userService;
        _leaveApplicationService = leaveApplicationService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<UserLookupResponse>>> GetUsers(
        CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetUsersAsync(cancellationToken));
    }

    [HttpGet("leave-types")]
    public async Task<ActionResult<IReadOnlyList<LeaveTypeOptionResponse>>> GetLeaveTypes(
        CancellationToken cancellationToken)
    {
        return Ok(await _leaveApplicationService.GetLeaveTypeOptionsAsync(cancellationToken));
    }
}
