using Francois.FunctionApp.Services;
using Functions.Config;
using Functions.Services;
using Functions.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Monitor.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, s) =>
    {
        s.AddDbContext<ApplicationDbContext>(o =>
        {
            var connectionString = context.Configuration.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connectionString))
                o.UseNpgsql();
            else
                o.UseNpgsql(connectionString,
                    o => o.EnableRetryOnFailure());
        });
        s.AddOptions<SiteOptions>()
            .Configure<IConfiguration>((siteOptions, configuration) =>
            {
                configuration.GetSection("RedCap").Bind(siteOptions);
            });
        s.AddTransient<SiteService>();
        s.AddTransient<IReportingService, ReportService>();
        
        var useRedCapData = context.Configuration.GetValue<bool>("UseRedCapData");
        if (useRedCapData) s.AddTransient<IDataService, RedCapSitesService>();
        else s.AddTransient<IDataService, DummyDataService>();
    })
    .Build();
    
host.Run();
