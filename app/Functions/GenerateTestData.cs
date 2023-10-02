using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions;

public static class GenerateTestData
{
    [Function("GenerateTestData")]
    public static void Run([TimerTrigger("0 */5 * * * *")] MyInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("GenerateTestData");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        
        // Open spreadsheet
        
        // Loop rows
        
        // Strongly typed rows
        
        // Get field type
        // Get validation parameters
        // Map special fields
        // Generate test data
        // Save spreadsheet
    }
}