using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost]
    [SwaggerOperation("Create a new study.")]
    [SwaggerResponse(200, Type = typeof(StudyModel[]))]
    [SwaggerResponse(400, "The study could not be validated with RedCap.")]
    [SwaggerResponse(400, "The user has already added the study.")]
    [SwaggerResponse(401, "User is not authorized.")]
    public async Task<ActionResult> Create(
        [SwaggerParameter("RedCap Study model.")] [FromForm]
        StudyModel model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return Unauthorized();
        }

        // Validate the study again with RedCap and compare them
        var redCapStudy = await _studyService.Validate(model.ApiKey);
        if (!redCapStudy.Equals(model))
        {
            return BadRequest("We could not revalidate the study with RedCap.");
        }

        try
        {
            return Ok(await _studyService.Create(model, userId));
        }
        catch (DbUpdateException e)
        {
            return BadRequest("You have already added this study.");
        }
    }

    [HttpPost("validate")]
    [SwaggerOperation("Validate a studies API Key.")]
    [SwaggerResponse(200, Type = typeof(StudyModel[]))]
    [SwaggerResponse(401, "User is not authorized.")]
    [SwaggerResponse(401, "The Api Key is not authorized with RedCap.")]
    public async Task<ActionResult> Validate(
        [SwaggerParameter("RedCap API Key.")] [FromForm]
        string apiKey
    )
    {
        try
        {
            var model = await _studyService.Validate(apiKey);
            return Ok(model);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(401, ex.Message);
        }
    }
}