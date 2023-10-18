using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Services;
using Monitor.Services.SyntheticData;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation("Generate synthetic data from a data dictionary.")]
    [SwaggerResponse(200, "Synthetic Data was generated", typeof(string))]
    [SwaggerResponse(400, "File is not a valid data dictionary.")]
    [SwaggerResponse(500, "Synthetic data failed to generate or upload.")]
    public async Task<ActionResult<string>> Generate(
        [SwaggerParameter("RedCap data dictionary file.")] [FromForm] IFormFile file, 
        [SwaggerParameter("Event name to generate for.")] [FromForm] string eventName)
    {
        try
        {
            _syntheticData.Validate(file);
            var syntheticData = _syntheticData.Generate(file, eventName);
            var name = await _azureStorageService.UploadSpreadsheet(syntheticData);
            
            return Ok(new { name });
        }
        catch (InvalidDataException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
    
    [HttpGet("file")]
    [SwaggerOperation("Download file.")]
    [SwaggerResponse(200, "File download")]
    [SwaggerResponse(404, "File not found.")]
    public async Task<IActionResult> Get([SwaggerParameter("Name of the file to get.")] string name)
    {
        var fileBytes = await _azureStorageService.Get(name);
        return File(fileBytes, "application/octet-stream", name);
    }
}