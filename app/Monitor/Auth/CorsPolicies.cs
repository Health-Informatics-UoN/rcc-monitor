using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Monitor.Auth;

public static class CorsPolicies
{
    /// <summary>
    /// Creates a CORS policy that allows requests from the frontend URL loaded in config.
    /// </summary>
    /// <param name="configuration">The configuration containing the frontend URL.</param>
    /// <returns>A CORS policy that restricts cross-origin requests to the specified URL.</returns>
    public static CorsPolicy AllowFrontendApp(IConfiguration configuration)
    {
        var frontendAppUrl = configuration["FrontendAppUrl"];

        return new CorsPolicyBuilder()
            .WithOrigins(frontendAppUrl ?? string.Empty)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .Build();
    }
}
