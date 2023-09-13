using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monitor.Auth;
using Monitor.Models;
using Monitor.Services;

namespace Monitor.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }
    
    // TODO: Fix when auth is reimplemented
    // [AllowAnonymous]
    // [Authorize(Policy=nameof(AuthPolicies.CanAccessReports))]
    [HttpGet]
    public async Task<ActionResult<List<ReportModel>>> ListAll()
        => Ok(await _reportService.List());

    // [Authorize(nameof(AuthPolicies.CanAccessReports))]
    // TODO: Fix when auth is reimplemented
    [AllowAnonymous]
    [HttpPost("SendSummary")]
    public async Task<ActionResult<List<ReportModel>>> SendSummary()
    {
        await _reportService.SendSummary();
        return NoContent();
    }
    
}
