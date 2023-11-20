using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Models.Users;
using Monitor.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.StudyManagement)]
[Route("api/[controller]")]
[Authorize(nameof(AuthPolicies.CanViewUsers))]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("unaffiliated")]
    [SwaggerOperation("Get a list of users who are not affiliated with a study")]
    [SwaggerResponse(200, "List of users.")]
    [SwaggerResponse(403, "User is not authorized.")]
    [SwaggerResponse(404, "Study not found.")]
    public async Task<IActionResult> Unaffiliated(UnaffiliatedQuery model)
    {
        try
        {
            return Ok(await _userService.GetUnaffiliatedUsers(model));
        }
        catch (KeyNotFoundException)
        {
            return BadRequest("Study not found.");
        }
    }
}