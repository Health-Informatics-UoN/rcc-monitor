using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Monitor.Constants;

namespace Monitor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConfigController
{
    private readonly IFeatureManagerSnapshot _featureManager;

    public ConfigController(IFeatureManagerSnapshot featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index() => new JsonResult(new
    {
        siteMonitoringEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.SiteMonitoring)
    });
}