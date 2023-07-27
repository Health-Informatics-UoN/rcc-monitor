using Francois.FunctionApp.Config;
using Francois.FunctionApp.Services;
using Francois.FunctionApp.Services.Contracts;
using Francois.FunctionApp.StartupHelpers.ConfigureServicesExtensions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Francois.FunctionApp.Startup))]

namespace Francois.FunctionApp;

public class Startup : FunctionsStartup
{
    private IConfiguration _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddOptions<SiteOptions>()
            .Configure<IConfiguration>((siteOptions, configuration) =>
            {
                configuration.GetSection("RedCap").Bind(siteOptions);
            });
        
        ConfigureServices(builder.Services).BuildServiceProvider();
    }

    private IServiceCollection ConfigureServices(IServiceCollection services)
    {
        _configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        
        services
            .AddEmailSender(_configuration)
            .AddTransient<SiteService>()
            .AddHttpClient();

        var useRedCapData = _configuration.GetValue<bool>("UseRedCapData");
        var useEmailReports = _configuration.GetValue<bool>("UseEmailReports");

        if (useRedCapData) services.AddTransient<IDataService, RedCapSitesService>();
        else services.AddTransient<IDataService, DummyDataService>();
        
        if (useEmailReports) services.AddTransient<IReportingService, EmailService>();
        else services.AddTransient<IReportingService, PlannerService>();
        
        return services;
    }
}