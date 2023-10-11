using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Monitor.Auth;
using Monitor.Constants;
using Monitor.Models;
using Monitor.Services;

namespace Monitor.Controllers;

[ApiController]
[FeatureGate(FeatureFlags.SiteMonitoring)]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [Authorize(nameof(AuthPolicies.CanViewSiteReports))]
    [HttpGet]
    public async Task<ActionResult<List<ReportModel>>> ListAll()
        => Ok(await _reportService.List());

    [Authorize(nameof(AuthPolicies.CanSendSummary))]
    [HttpPost("SendSummary")]
    public async Task<ActionResult<List<ReportModel>>> SendSummary()
    {
        await _reportService.SendSummary();
        return NoContent();
    }

}
