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

    // [Authorize(nameof(AuthPolicies.CanGenerateSyntheticData))]
    [AllowAnonymous]
    [HttpPost("generate")]
    public async Task<ActionResult<string>> Generate([FromForm] IFormFile file)
    {
        // TODO: Add server side validation.
        var generatedCsv = _syntheticData.Generate(file);
        
        HttpContext.Session.Set("export", generatedCsv);
        
        return Ok(new KeyValuePair<bool, string>(true, Url.Action("TempExport", "SyntheticData")));
    }

    // [Authorize(nameof(AuthPolicies.CanGenerateSyntheticData))]
    [AllowAnonymous]
    [HttpGet("tempexport")]
    public ActionResult TempExport()
    {
        var bytes = HttpContext.Session.Get("export");
        return File(bytes, "text/csv", "generated-data.csv");
    }
    
    
}