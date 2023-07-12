using Francois.FunctionApp.Config;
using Francois.FunctionApp.services;
using Francois.FunctionApp.services.Contracts;
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
        services
            .AddTransient<SiteService>()
            .AddHttpClient();
        
        _configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        
        var useRedCapData = _configuration.GetValue<bool>("UseRedCapData");
        
        if (useRedCapData) services.AddTransient<IDataService, RedCapSitesService>();
        else services.AddTransient<IDataService, DummyDataService>();

        return services;
    }
}