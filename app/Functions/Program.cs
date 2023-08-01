using Francois.FunctionApp.Services;
using Functions.Config;
using Functions.Services;
using Functions.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(((context, s) =>
    {
        s.AddOptions<SiteOptions>()
            .Configure<IConfiguration>((siteOptions, configuration) =>
            {
                configuration.GetSection("RedCap").Bind(siteOptions);
            });
        s.AddTransient<SiteService>();
        s.AddTransient<IReportingService, EmailService>();
        
        var useRedCapData = context.Configuration.GetValue<bool>("UseRedCapData");
        if (useRedCapData) s.AddTransient<IDataService, RedCapSitesService>();
        else s.AddTransient<IDataService, DummyDataService>();
    }))
    .Build();
    
host.Run();
