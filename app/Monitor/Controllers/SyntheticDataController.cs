using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Constants;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.SyntheticData)]
[Route("api/[controller]")]
[Authorize]
public class SyntheticDataController : ControllerBase
{
    [HttpPost("synthetic-data")]
    public async Task<ActionResult<IFormFile>> GenerateSyntheticData(IFormFile file)
    {
        throw new NotImplementedException();
    }
}