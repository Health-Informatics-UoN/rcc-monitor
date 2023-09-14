using System.Security.Claims;
using ClacksMiddleware.Extensions;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Monitor.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monitor.Data;
using Monitor.Data.Entities.Identity;

using Monitor.Config;
using Monitor.Services;
using Monitor.Constants;
using UoN.AspNetCore.VersionMiddleware;
using Monitor.Auth;

var b = WebApplication.CreateBuilder(args);

#region Configure Services

b.Services.AddHttpLogging(o =>
{
  o.RequestHeaders.Add("Authorization");
  o.RequestHeaders.Add("Referer");
  o.ResponseHeaders.Add("WWW-Authenticate");
});

b.Services.AddKeycloakAuthentication(b.Configuration);
b.Services.AddAuthorization(options =>
  {
    // options.AddPolicy("RequireWorkspaces", builder =>
    // {
    //   builder.RequireProtectedResource("workspaces", "workspaces:read") // HTTP request to Keycloak to check protected resource
    //     .RequireRealmRoles("User") // Realm role is fetched from token
    //     .RequireResourceRoles("Admin"); // Resource/Client role is fetched from token
    // });
  })
  .AddKeycloakAuthorization(b.Configuration);

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

// Identity
// b.Services
//   .AddIdentity<ApplicationUser, IdentityRole>(
//     o => o.SignIn.RequireConfirmedEmail = true)
//   .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
//   .AddEntityFrameworkStores<ApplicationDbContext>()
//   .AddDefaultTokenProviders();

b.Services
  .AddApplicationInsightsTelemetry()
  .ConfigureApplicationCookie(AuthConfiguration.IdentityCookieOptions)
  // .AddAuthorization(AuthConfiguration.AuthOptions)
  .Configure<RegistrationOptions>(b.Configuration.GetSection("Registration"))
  .Configure<UserAccountOptions>(b.Configuration.GetSection("UserAccounts"))

  .AddEmailSender(b.Configuration)

  // .AddTransient<UserService>()
  // .AddTransient<RegistrationRuleService>()
  .AddTransient<ReportService>();

b.Services.AddSwaggerGen();

//TODO: Configure CORS for client app only
b.Services.AddCors(options =>
{
  options.AddPolicy("AllowClientApp",
    builder =>
      builder.AllowAnyOrigin()
        .AllowAnyMethod()
  );
});

#endregion

#region seeding

var app = b.Build();

// Do data seeding isolated from the running of the app
// using (var scope = app.Services.CreateScope())
// {
//   var db = scope.ServiceProvider
//     .GetRequiredService<ApplicationDbContext>();
//
//   var roles = scope.ServiceProvider
//     .GetRequiredService<RoleManager<IdentityRole>>();
//
//   var registrationRule = scope.ServiceProvider
//     .GetRequiredService<RegistrationRuleService>();
//
//   var config = scope.ServiceProvider
//     .GetRequiredService<IConfiguration>();
//
//   var seeder = new DataSeeder(db, roles, registrationRule);
//   
//   await seeder.SeedRoles();
//   await seeder.SeedRegistrationRules(config);
//   await seeder.SeedReportTypes();
//   await seeder.SeedInstanceTypes();
//   await seeder.SeedReportStatus();
// }

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
app.UseCors("AllowClientApp");
app.UseHttpLogging();
#endregion

#region Endpoint Routing

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (ClaimsPrincipal user) =>
{
  app.Logger.LogInformation(user.Identity.Name);
}).RequireAuthorization();

// Endpoints

app.MapControllers();

app.MapFallbackToFile("index.html").AllowAnonymous();

#endregion

app.Run();
