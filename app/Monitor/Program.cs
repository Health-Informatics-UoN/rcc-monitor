using ClacksMiddleware.Extensions;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk.Admin;
using Monitor.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.FeatureManagement;
using Monitor.Data;

using Monitor.Services;
using Monitor.Constants;
using UoN.AspNetCore.VersionMiddleware;
using Monitor.Auth;
using Monitor.Config;
using Monitor.Services.SyntheticData;

var b = WebApplication.CreateBuilder(args);

#region Configure Services

// KeyCloak Identity
b.Services.AddKeycloakAuthentication(b.Configuration);
b.Services.AddAuthorization(AuthConfiguration.AuthOptions)
  .AddKeycloakAuthorization(b.Configuration);
b.Services.AddKeycloakAdminHttpClient(b.Configuration);
b.Services.AddOptions().Configure<KeycloakOptions>(b.Configuration.GetSection("Keycloak"));

// CORS
b.Services.AddCors(AuthConfiguration.CorsOptions(b.Configuration));

// MVC
b.Services
  .AddControllersWithViews()
  .AddJsonOptions(DefaultJsonOptions.Configure);

// EF
b.Services
  .AddDbContext<ApplicationDbContext>(o =>
    {
      // migration bundles don't like null connection strings (yet)
      // https://github.com/dotnet/efcore/issues/26869
      // so if no connection string is set we register without one for now.
      // if running migrations, `--connection` should be set on the command line
      // in real environments, connection string should be set via config
      // all other cases will error when db access is attempted.
      var connectionString = b.Configuration.GetConnectionString("Default");
      if (string.IsNullOrWhiteSpace(connectionString))
        o.UseNpgsql();
      else
        o.UseNpgsql(connectionString,
          o => o.EnableRetryOnFailure());
    });

b.Services
  .AddApplicationInsightsTelemetry()
  .AddEmailSender(b.Configuration)
  .AddTransient<ReportService>()
  .AddTransient<SyntheticDataService>()
  .AddTransient<StudyService>()
  .AddTransient<UserService>();

b.Services.AddSwaggerGen(c =>
{
  c.EnableAnnotations();
});

// Blob storage
b.Services.AddAzureClients(builder =>
{
  builder.AddBlobServiceClient(b.Configuration.GetConnectionString("AzureStorage"));
});
b.Services.AddScoped<AzureStorageService>();

b.Services.AddFeatureManagement();

b.Services.AddOptions().Configure<RedCapOptions>(b.Configuration.GetSection("RedCap"));
#endregion

#region seeding

var app = b.Build();

// Do data seeding isolated from the running of the app
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider
    .GetRequiredService<ApplicationDbContext>();

  var seeder = new DataSeeder(db);

  await seeder.SeedReportTypes();
  await seeder.SeedInstanceTypes();
  await seeder.SeedReportStatus();
}

#endregion

#region Configure Pipeline

app.GnuTerryPratchett();

if (!app.Environment.IsDevelopment())
{
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseVersion();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(nameof(CorsPolicies.AllowFrontendApp));
#endregion

#region Endpoint Routing

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();

app.MapFallbackToFile("index.html").AllowAnonymous();

#endregion

app.Run();
