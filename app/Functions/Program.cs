using Francois.FunctionApp.Services;
using Functions.Config;
using Functions.Services;
using Functions.Services.Contracts;
using Functions.Services.SyntheticData;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Monitor.Data;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, s) =>
    {
        // Add identity token management
        var identityConfig = context.Configuration.GetSection("Identity").Get<IdentityOptions>();
        s.AddAccessTokenManagement(options =>
        {
            options.Client.Clients.Add("identity", new ClientCredentialsTokenRequest
            {
                Address = identityConfig.Address,
                ClientId = identityConfig.ClientId,
                ClientSecret = identityConfig.Secret
            });
        });

        // Adds a named client that uses the token management 
        var apiConfig = context.Configuration.GetSection("RedCap").Get<SiteOptions>();
        s.AddClientAccessTokenHttpClient("client", configureClient: client =>
        {
            client.BaseAddress = new Uri(apiConfig.ApiUrl);
        });
        
        s.AddDbContext<ApplicationDbContext>(o =>
        {
            var connectionString = context.Configuration.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(connectionString))
                o.UseNpgsql();
            else
                o.UseNpgsql(connectionString,
                    o => o.EnableRetryOnFailure());
        });
        s.AddOptions()
            .Configure<SiteOptions>(context.Configuration.GetSection("RedCap"));

        s.AddTransient<SyntheticDataService>();
        s.AddTransient<SiteService>();
        s.AddTransient<IReportingService, ReportService>();
        s.AddHttpClient();
        
        var useRedCapData = context.Configuration.GetValue<bool>("UseRedCapData");
        if (useRedCapData) s.AddTransient<IDataService, RedCapSitesService>();
        else s.AddTransient<IDataService, DummyDataService>();
    })
    .Build();
    
host.Run();
