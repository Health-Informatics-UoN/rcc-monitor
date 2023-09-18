using Functions.Services.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions;

public class SendSummaryEmail
{
    private readonly IReportingService _reportingService;

    public SendSummaryEmail(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }
    
    [Function("SendSummaryEmail")]
    public async Task Run([TimerTrigger("0 0 11 * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("SendSummaryEmail");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        
        // Send summary request
        await _reportingService.SendSummary();

    }
}
