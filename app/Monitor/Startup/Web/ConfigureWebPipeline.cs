using ClacksMiddleware.Extensions;
using Monitor.Auth;
using UoN.AspNetCore.VersionMiddleware;

namespace Monitor.Startup.Web;

public static class ConfigureWebPipeline
{
    /// <summary>
    /// Configure the HTTP Request Pipeline for an ASP.NET Core app
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseWebPipeline(this WebApplication app)
    {
        app.GnuTerryPratchett();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseVersion();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(nameof(CorsPolicies.AllowFrontendApp));

        // Routing before Auth
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // API Controllers
        app.MapControllers();

        app.MapFallbackToFile("index.html").AllowAnonymous();
        return app;

    }
}