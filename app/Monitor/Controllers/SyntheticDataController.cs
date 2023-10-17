using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Exceptions;
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
    public async Task<ActionResult<string>> Generate([FromForm] IFormFile file, [FromForm] string eventName)
    {
        try
        {
            _syntheticData.Validate(file);
            var syntheticData = _syntheticData.Generate(file, eventName);
            var url = await _azureStorageService.UploadSpreadsheet(syntheticData, Request.Scheme, Request.Host.ToString());
            return Ok(new { url });
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (DataGenerationException e)
        {
            return Problem(detail: e.Message, statusCode: 500);
        }
        catch (DataUploadException e)
        {
            return Problem(detail: e.Message, statusCode: 500);
        }
        catch (Exception e)
        {
            return Problem();
        }
    }


    [HttpGet("file")]
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