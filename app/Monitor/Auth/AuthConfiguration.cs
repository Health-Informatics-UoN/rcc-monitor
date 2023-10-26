using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Monitor.Auth;

public static class AuthConfiguration
{
  public static readonly Action<AuthorizationOptions> AuthOptions =
    b =>
    {
      // Nothing in SargAssure (at this time) should use [AllowAnonymous]
      // This is used when `[Authorize]` is provided with no specific policy / config
      b.DefaultPolicy = AuthPolicies.IsAuthenticatedUser;

      b.AddPolicy(nameof(AuthPolicies.CanViewSiteReports), AuthPolicies.CanViewSiteReports);
      b.AddPolicy(nameof(AuthPolicies.CanSendSummary), AuthPolicies.CanSendSummary);
      b.AddPolicy(nameof(AuthPolicies.CanGenerateSyntheticData), AuthPolicies.CanGenerateSyntheticData);
      b.AddPolicy(nameof(AuthPolicies.CanViewStudies), AuthPolicies.CanViewStudies);
      b.AddPolicy(nameof(AuthPolicies.CanViewAllStudies), AuthPolicies.CanViewAllStudies);
    };

  public static Action<CorsOptions> CorsOptions(IConfiguration configuration)
  {
    return b =>
    {
      b.AddPolicy(nameof(CorsPolicies.AllowFrontendApp), CorsPolicies.AllowFrontendApp(configuration));
    };
  }

}
