using Functions.Services.SyntheticData;
using Microsoft.Azure.Functions.Worker;
using OfficeOpenXml;

namespace Functions;

public class GenerateTestData
{
    private readonly SyntheticDataService _syntheticDataService;
    
    public GenerateTestData(SyntheticDataService syntheticDataService)
    {
        _syntheticDataService = syntheticDataService;
    }
    
    [Function("GenerateTestData")]
    public void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] MyInfo myTimer, FunctionContext context)
    {
        // TODO: Get this licensing working from config
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        // _syntheticDataService.Generate();
        _syntheticDataService.GenerateFolder();
    }
}