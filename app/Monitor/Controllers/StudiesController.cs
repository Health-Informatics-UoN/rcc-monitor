using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Services;
using Monitor.Shared.Exceptions;
using Monitor.Shared.Models.Studies;
using Monitor.Shared.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.StudyManagement)]
[Route("api/[controller]")]
[Authorize(nameof(AuthPolicies.CanViewStudies))]
public class StudiesController(StudyService studyService, StudyPermissionService studyPermissionService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get a list of Studies.")]
    [SwaggerResponse(200, Type = typeof(StudyPartialModel[]))]
    [SwaggerResponse(403, "User is not authorized.")]
    public async Task<ActionResult<List<StudyPartialModel>>> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (User.HasClaim("role", SitePermissionClaims.ViewAllStudies))
        {
            return Ok(await studyService.List());
        }

        return userId is not null ? Ok(await studyService.List(userId)) : Forbid();
    }    
    
    [HttpGet("{id:int}")]
    [SwaggerOperation("Get a study.")]
    [SwaggerResponse(200, Type = typeof(StudyPartialModel))]
    [SwaggerResponse(403, "User is not authorized.")]
    [SwaggerResponse(404, "Study not found.")]
    public async Task<ActionResult<StudyPartialModel>> Get(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (User.HasClaim("role", SitePermissionClaims.ViewAllStudies))
        {
            try
            {
                var study = await studyService.Get(id);
                return Ok(study);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Study not found.");
            }
        }

        try
        {
            return userId is not null ? Ok(await studyService.Get(id, userId)) : Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Study not found.");
        }
    }

    [HttpPost]
    [SwaggerOperation("Create a new study.")]
    [SwaggerResponse(200, Type = typeof(StudyModel[]))]
    [SwaggerResponse(400, "The study could not be validated with RedCap.")]
    [SwaggerResponse(400, "The user has already added the study.")]
    [SwaggerResponse(401, "User is not authorized.")]
    public async Task<ActionResult> Create(
        [SwaggerParameter("RedCap Study model.")]
        StudyModel model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return Unauthorized();
        }

        // Validate the study again with RedCap and compare them
        // It's necessary to double check this to be certain this is the study we want.
        var redCapStudy = await studyService.Validate(model.ApiKey);
        if (redCapStudy.Id != model.Id || redCapStudy.Name != model.Name || redCapStudy.ApiKey != model.ApiKey)
        {
            return BadRequest("We could not revalidate the study with RedCap.");
        }

        try
        {
            return Ok(await studyService.Create(model, userId));
        }
        catch (DbUpdateException)
        {
            return BadRequest("You have already added this study.");
        }
    }

    [HttpPost("validate")]
    [SwaggerOperation("Validate a studies API Key and permissions.")]
    [SwaggerResponse(200, Type = typeof(StudyModel[]))]
    [SwaggerResponse(401, "User is not authorized.")]
    [SwaggerResponse(401, "The Api Key is not authorized with RedCap.")]
    public async Task<ActionResult> Validate(
        [SwaggerParameter("RedCap API Key.")]
        StudyToken model
    )
    {
        try
        {
            var response = await studyService.Validate(model.ApiKey);
            await studyPermissionService.ValidatePermissions(response);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(401, ex.Message);
        }
        catch (Exception ex) when (ex is MissingPermissionsException or ExtraPermissionsException)
        {
            return StatusCode(403, ex.Message);
        }
    }
    
    [HttpPut("{id:int}")]
    [Authorize(nameof(AuthPolicies.CanUpdateStudies))]
    [SwaggerOperation("Update a Study.")]
    [SwaggerResponse(200, "Study updated successfully.")]
    [SwaggerResponse(400, "Invalid input format")]
    [SwaggerResponse(403, "User is not authorized.")]
    [SwaggerResponse(404, "Study not found.")]
    public async Task<ActionResult> Update(int id, StudyPartialModel model)
    {
        try
        {
            var response = await studyService.Update(id, model, User);
            return Ok(response);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("{redCapId}")]
    [Authorize(nameof(AuthPolicies.CanDeleteStudies))]
    [SwaggerOperation("Delete a Study by RedCapId.")]
    [SwaggerResponse(204, "Study deleted successfully.")]
    [SwaggerResponse(404, "Study not found.")]
    [SwaggerResponse(403, "User is not authorized.")]
    public async Task<ActionResult> Delete(int redCapId)
    {
        try
        {
            await studyService.DeleteStudy(redCapId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}