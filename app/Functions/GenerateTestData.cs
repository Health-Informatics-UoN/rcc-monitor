using Functions.Services;
using Microsoft.Azure.Functions.Worker;
using OfficeOpenXml;

namespace Functions;

public class GenerateTestData
{
    private readonly DataGenerationService _dataGenerationService;
    
    public GenerateTestData(DataGenerationService dataGenerationService)
    {
        _dataGenerationService = dataGenerationService;
    }
    
    [Function("GenerateTestData")]
    public void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        // TODO: Get this licensing working from config
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        _dataGenerationService.GenerateData();

        // Loop rows
        
        // Strongly typed rows
        
        // Get field type
        // Get validation parameters
        // Map special fields
        // Generate test data
        
        // Save spreadsheet
        // the output file
        
    }
}