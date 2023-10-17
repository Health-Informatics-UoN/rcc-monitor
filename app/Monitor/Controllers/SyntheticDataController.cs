using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Services;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.SyntheticData)]
[Route("api/[controller]")]
[Authorize(nameof(AuthPolicies.CanGenerateSyntheticData))]
public class SyntheticDataController : ControllerBase
{
    private readonly AzureStorageService _azureStorageService;
    private 
    public SyntheticDataController(AzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;
    }
    
    [HttpPost("generate")]
    public async Task<ActionResult<string>> Generate([FromForm] IFormFile file, [FromForm] string eventName)
    {
        // TODO: Upload to storage
        var generatedCsv = _syntheticData.Generate(file, eventName);
        // return Task.FromResult<ActionResult<string>>(File(generatedCsv, "text/csv", "generated-data.csv"));
        return Ok();
    }
}