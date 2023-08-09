using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monitor.Auth;
using Monitor.Models;
using Monitor.Services;

namespace Monitor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [Authorize(nameof(AuthPolicies.CanAccessReports))]
    [HttpGet]
    public async Task<ActionResult<List<ReportModel>>> ListAll()
        => Ok(await _reportService.List());

    [Authorize(nameof(AuthPolicies.CanAccessReports))]
    [HttpPost("conflicting-site")]
    public async Task<IActionResult> ConflictingSite(List<ReportModel> reports)
    {
        await _reportService.ConflictingSiteTriggerReport(reports);
        return NoContent();
    }
    
    [Authorize(nameof(AuthPolicies.CanAccessReports))]
    [HttpPost("conflicting-site-name")]
    public async Task<IActionResult> ConflictingSiteName(List<(ReportModel, ReportModel)> reports)
    {
        await _reportService.ConflictingSiteNameTriggerReport(reports);
        return NoContent();
    }
    
    [Authorize(nameof(AuthPolicies.CanAccessReports))]
    [HttpPost("conflicting-site-parent")]
    public async Task<IActionResult> ConflictingSiteParent(List<(ReportModel, ReportModel)> reports)
    {
        await _reportService.ConflictingSiteParentTriggerReport(reports);
        return NoContent();
    }
}