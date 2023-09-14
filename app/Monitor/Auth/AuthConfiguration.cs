using Microsoft.AspNetCore.Authorization;

namespace Monitor.Auth;

public static class AuthConfiguration
{
  public static readonly Action<AuthorizationOptions> AuthOptions =
    b =>
    {
      // Nothing in SargAssure (at this time) should use [AllowAnonymous]
      // This is used when `[Authorize]` is provided with no specific policy / config
      b.DefaultPolicy = AuthPolicies.IsAuthenticatedUser;
      
      b.AddPolicy(nameof(AuthPolicies.IsSiteAdmin), AuthPolicies.IsSiteAdmin);
    };


}
