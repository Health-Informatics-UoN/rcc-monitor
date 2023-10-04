using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Functions;

public static class GenerateTestData
{
    [Function("GenerateTestData")]
    public static void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        var logger = context.GetLogger("GenerateTestData");
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        
        // TODO: Get this licensing working from config
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        // Open spreadsheet
        using var pck = new ExcelPackage();
        var sheet = pck.Workbook.Worksheets.Add("sheet");
        
        var file = new FileInfo("import_dictionary.csv");
        sheet.Cells["A1"].LoadFromText(file);

        // Loop rows
        
        // Strongly typed rows
        
        // Get field type
        // Get validation parameters
        // Map special fields
        // Generate test data
        
        // Save spreadsheet
        // the output file
        var output = new FileInfo(@"export.csv");
        // format with default parameters
        var format = new ExcelOutputTextFormat();
        sheet.Cells["A1:D5"].SaveToText(output, format);
    }
}