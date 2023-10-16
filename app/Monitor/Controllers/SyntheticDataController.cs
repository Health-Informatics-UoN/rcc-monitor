using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Services.SyntheticData;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.SyntheticData)]
[Route("api/[controller]")]
[Authorize(nameof(AuthPolicies.CanGenerateSyntheticData))]
public class SyntheticDataController : ControllerBase
{
    private readonly SyntheticDataService _syntheticData;
    
    public SyntheticDataController(SyntheticDataService syntheticDataService)
    {
        _syntheticData = syntheticDataService;
    }
    
    [HttpPost("generate")]
    public Task<ActionResult<string>> Generate([FromForm] IFormFile file, [FromForm] string eventName)
    {
        var generatedCsv = _syntheticData.Generate(file, eventName);
        return Task.FromResult<ActionResult<string>>(File(generatedCsv, "text/csv", "generated-data.csv"));
    }
}