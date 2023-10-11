using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
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
    
    [HttpPost("synthetic-data")]
    public Task<ActionResult<IFormFile>> GenerateSyntheticData(IFormFile file)
    {
        var generatedCsv = _syntheticData.Generate(file);
        return Task.FromResult<ActionResult<IFormFile>>(File(generatedCsv, "text/csv", "generated-data.csv"));
    }
}