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
[Authorize]
public class SyntheticDataController : ControllerBase
{
    private readonly SyntheticDataService _syntheticData;
    
    public SyntheticDataController(SyntheticDataService syntheticDataService)
    {
        _syntheticData = syntheticDataService;
    }

    [Authorize(nameof(AuthPolicies.CanGenerateSyntheticData))]
    [HttpPost("generate")]
    public Task<ActionResult<IFormFile>> Generate([FromForm] IFormFile file)
    {
        // TODO: Add server side validation.
        var generatedCsv = _syntheticData.Generate(file);
        return Task.FromResult<ActionResult<IFormFile>>(File(generatedCsv, "text/csv", "generated-data.csv"));
    }
    
}