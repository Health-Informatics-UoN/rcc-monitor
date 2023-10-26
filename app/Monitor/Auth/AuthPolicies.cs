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
  
  public static AuthorizationPolicy CanGenerateSyntheticData
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.GenerateSyntheticData)
      .Build();  
  
  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can view studies.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanViewStudies
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.ViewStudies)
      .Build();

  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can view all studies.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanViewAllStudies
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.ViewAllStudies)
      .Build();

}
