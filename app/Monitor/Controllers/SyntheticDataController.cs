using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Services;
using Monitor.Services.SyntheticData;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.SyntheticData)]
[Route("api/[controller]")]
[Authorize(nameof(AuthPolicies.CanGenerateSyntheticData))]
public class SyntheticDataController : ControllerBase
{
    private readonly AzureStorageService _azureStorageService;
    private readonly SyntheticDataService _syntheticData;
    public SyntheticDataController(AzureStorageService azureStorageService, SyntheticDataService syntheticData)
    {
        _azureStorageService = azureStorageService;
        _syntheticData = syntheticData;
    }
    
    [HttpPost("generate")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Generate([FromForm] IFormFile file, [FromForm] string eventName)
    {
        // TODO: Validation step.
        
        // Generate data
        try
        {
            var generatedCsv = _syntheticData.Generate(file, eventName);
            try
            {
                using var stream = new MemoryStream(generatedCsv);
                var filePath = $"{eventName}_{Guid.NewGuid()}.csv";
                var blobName = await _azureStorageService.Upload(filePath, stream);
                return Ok(Url.Action("Get", "SyntheticData", new { name = blobName }));
            }
            catch (Exception e)
            {
                return Problem();
            }
        }
        catch (Exception e)
        {
            return Problem();
        }
    }

    [HttpGet("file")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(string name)
    {
        var fileBytes = await _azureStorageService.Get(name);
        if (fileBytes == null)
        {
            return NotFound();
        }
        return File(fileBytes, "application/octet-stream", name);
    }
}