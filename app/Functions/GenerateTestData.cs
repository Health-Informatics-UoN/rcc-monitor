using Functions.Services.SyntheticData;
using Microsoft.Azure.Functions.Worker;

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
        _syntheticDataService.Generate();

        // _syntheticDataService.Generate("redcap-dictionaries/GenMalCarb__eConsent_.csv");
        // _syntheticDataService.GenerateFolder();
    }
}