using Microsoft.AspNetCore.Authorization;

using Keycloak.AuthServices.Authorization;

namespace Monitor.Auth;

public static class AuthPolicies
{
  public static AuthorizationPolicy IsAuthenticatedUser
    => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
  
  public static AuthorizationPolicy CanViewSiteReports
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.ViewSiteReports)
      .Build();  
  
  public static AuthorizationPolicy CanSendSummary
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.SendSummaryEmail)
      .Build();
  
}
