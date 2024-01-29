using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.FeatureManagement;
using Monitor.Auth;
using Monitor.Config;
using Monitor.Constants;
using Monitor.Data;
using Monitor.Data.Config;
using Monitor.Extensions;
using Monitor.Models.SyntheticData;
using Monitor.Services;
using Monitor.Services.SyntheticData;
using Monitor.Shared.Config;
using Monitor.Shared.Services;

namespace Monitor.Startup.Web;

public static class ConfigureWebServices
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder b)
    {
        // App Options Configuration
        b.Services.AddOptions().Configure<KeycloakOptions>(b.Configuration.GetSection("Keycloak"));
        b.Services.AddOptions().Configure<RedCapOptions>(b.Configuration.GetSection("RedCap"));
        b.Services.AddOptions().Configure<FieldMappings>(b.Configuration.GetSection("SyntheticDataMapping"));

        // Map the Admin Client options private properties. 
        var adminClientOptions = b
            .Configuration
            .GetSection(KeycloakAdminClientOptions.Section)
            .Get<KeycloakAdminClientOptions>(opt => opt.BindNonPublicProperties = true);

        // KeyCloak Identity
        b.Services.AddKeycloakAuthentication(b.Configuration);
        b.Services.AddAuthorization(AuthConfiguration.AuthOptions)
            .AddKeycloakAuthorization(b.Configuration);
        b.Services.AddKeycloakAdminHttpClient(adminClientOptions ?? throw new InvalidOperationException());

        // CORS
        b.Services.AddCors(AuthConfiguration.CorsOptions(b.Configuration));

        // MVC
        b.Services
            .AddControllersWithViews()
            .AddJsonOptions(DefaultJsonOptions.Configure);

        // OpenAPI
        b.Services.AddSwaggerGen(c => { c.EnableAnnotations(); });

        // App insights
        b.Services
            .AddApplicationInsightsTelemetry();

        // Blob storage
        b.Services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(b.Configuration.GetConnectionString("AzureStorage"));
        });
        b.Services.AddScoped<AzureStorageService>();

        b.Services.AddFeatureManagement();

        // EF
        b.Services.AddDataDbContext(b.Configuration);

        // App specific services 
        b.Services
            .AddEmailSender(b.Configuration)
            .AddTransient<IRedCapStudyService, RedCapStudyService>()
            .AddTransient<ReportService>()
            .AddTransient<SyntheticDataService>()
            .AddTransient<StudyService>()
            .AddTransient<StudyPermissionService>()
            .AddTransient<UserService>()
            .AddTransient<ConfigService>();

        return b;
    }
}