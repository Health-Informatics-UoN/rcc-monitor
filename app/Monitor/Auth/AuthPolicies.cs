using Microsoft.AspNetCore.Authorization;

using Keycloak.AuthServices.Authorization;

namespace Monitor.Auth;

public static class AuthPolicies
{
  public static AuthorizationPolicy IsAuthenticatedUser
    => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
  
  public static AuthorizationPolicy IsSiteAdmin
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(Roles.SiteAdmin)
      .Build();
  
}
