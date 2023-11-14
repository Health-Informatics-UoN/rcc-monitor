using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Models;
using Monitor.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Monitor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConfigController : ControllerBase
{
    private readonly ConfigService _configService;
    private readonly IFeatureManagerSnapshot _featureManager;

    public ConfigController(ConfigService configService, IFeatureManagerSnapshot featureManager)
    {
        _configService = configService;
        _featureManager = featureManager;
    }

    [HttpGet("flags")]
    public async Task<IActionResult> GetFeatureFlags() => new JsonResult(new
    {
        siteMonitoringEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.SiteMonitoring),
        syntheticDataEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.SyntheticData),
        studyManagementEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.StudyManagement)
    });
    
    [HttpGet]
    [SwaggerOperation("Get a list of Config.")]
    [SwaggerResponse(200, Type = typeof(ConfigModel[]))]
    [SwaggerResponse(403, "User is not authorized.")]
    public async Task<ActionResult<IEnumerable<ConfigModel>>> Index() 
        => Ok(await _configService.List());

    [Authorize(nameof(AuthPolicies.CanEditConfig))]
    [HttpPut]
    [SwaggerOperation("Update a Config.")]
    [SwaggerResponse(204, "Config updated successfully.")]
    [SwaggerResponse(400, "Config value provided not valid")]
    [SwaggerResponse(403, "User is not authorized.")]
    [SwaggerResponse(404, "Config not found.")]
    public async Task<ActionResult> Edit(string key, ConfigModel configModel)
    {
        if (key == "RandomisationJobFrequency" && !TimeSpan.TryParse(configModel.Value, out _))
        {
            return BadRequest("The value provided is not valid");
        }
        try
        {
            await _configService.Edit(key, configModel);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}