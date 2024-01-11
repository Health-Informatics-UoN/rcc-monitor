using Functions.Config;
using Functions.Services;
using Functions.Services.Contracts;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Monitor.Data;
using Monitor.Data.Config;
using Monitor.Shared.Config;
using Monitor.Shared.Services;

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
                Address = identityConfig?.Address,
                ClientId = identityConfig?.ClientId,
                ClientSecret = identityConfig?.Secret
            });
        });

        // Adds a named client that uses the token management 
        var apiConfig = context.Configuration.GetSection("Backend").Get<SiteOptions>();
        s.AddClientAccessTokenHttpClient("client", configureClient: client =>
        {
            client.BaseAddress = new Uri(apiConfig?.ApiUrl ?? throw new InvalidOperationException());
        });
        

        s.AddDataDbContext(context.Configuration);
        s.AddOptions()
            .Configure<RedCapOptions>(context.Configuration.GetSection("RedCap"));
            
        s.AddTransient<SiteService>();
        s.AddTransient<IReportingService, ReportService>();
        s.AddTransient<RedCapStudyService>();
        s.AddTransient<StudyCapacityService>();
        s.AddHttpClient();
        
        var useRedCapData = context.Configuration.GetValue<bool>("UseRedCapData");
        if (useRedCapData) s.AddTransient<IDataService, RedCapSitesService>();
        else s.AddTransient<IDataService, PlaceholderDataService>();
    })
    .Build();
    
host.Run();
