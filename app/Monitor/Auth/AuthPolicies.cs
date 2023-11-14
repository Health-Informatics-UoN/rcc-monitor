using Microsoft.AspNetCore.Authorization;

using Keycloak.AuthServices.Authorization;

namespace Monitor.Auth;

public static class AuthPolicies
{
  public static AuthorizationPolicy IsAuthenticatedUser
    => new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can generate view site reports.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanViewSiteReports
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.ViewSiteReports)
      .Build();

  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can send summary email.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanSendSummary
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.SendSummaryEmail)
      .Build();
  
  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can generate synthetic data.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
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
  
  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can delete studies.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanDeleteStudies
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.DeleteStudies)
      .Build();  
  
  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can update studies.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanUpdateStudies
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.UpdateStudies)
      .Build();  
  
  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can remove a study users.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanRemoveStudyUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.RemoveStudyUsers)
      .Build();

  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can view users.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanViewUsers
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.ViewUsers)
      .Build();
  
  /// <summary>
  /// Requires a request <see cref="IsAuthenticatedUser"/>,
  /// and can edit config.
  /// </summary>
  /// <returns>A new <see cref="AuthorizationPolicy"/> built from the requirements.</returns>
  public static AuthorizationPolicy CanEditConfig
    => new AuthorizationPolicyBuilder()
      .Combine(IsAuthenticatedUser)
      .RequireRealmRoles(SitePermissionClaims.EditConfig)
      .Build();
}
