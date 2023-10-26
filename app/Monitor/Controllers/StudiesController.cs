using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Models;
using Monitor.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.StudyManagement)]
[Route("api/[controller]")]
[Authorize(nameof(AuthPolicies.CanViewStudies))]
public class StudiesController : ControllerBase
{
    private readonly StudyService _studyService;
    
    public StudiesController(StudyService studyService)
    {
        _studyService = studyService;
    }

    [HttpGet]
    [SwaggerOperation("Get a list of Studies.")]
    [SwaggerResponse(200, Type = typeof(StudyPartialModel[]))]
    [SwaggerResponse(403, "User is not authorized.")]
    public async Task<ActionResult<List<StudyPartialModel>>> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (User.HasClaim("role", SitePermissionClaims.ViewAllStudies))
        {
            return Ok(await _studyService.List());
        }
        
        return userId is not null ? Ok(await _studyService.List(userId)) : Forbid();
    }
}