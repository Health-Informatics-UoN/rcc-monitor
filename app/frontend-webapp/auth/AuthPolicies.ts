import { Permissions } from "@/auth/Permissions";
import { AuthorizationPolicyBuilder } from "@/lib/auth";

export class AuthorizationPolicies {
  public static readonly IsAuthenticatedUser = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

  /**
   * Requires a user is Authenticated and can View Site Reports
   */
  public static readonly CanViewSiteReports = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.ViewSiteReports)
    .Build();

  /**
   * Requires a user is Authenticated and can Generate Synthetic Data
   */
  public static readonly CanGenerateSyntheticData =
    new AuthorizationPolicyBuilder()
      .Combine(AuthorizationPolicies.IsAuthenticatedUser)
      .RequirePermissions(Permissions.GenerateSyntheticData)
      .Build();

  /**
   * Requires a user is Authenticated and can View Studies
   */
  public static readonly CanViewStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.ViewStudies)
    .Build();

  /**
   * Requires a user is Authenticated and can Delete Studies
   */
  public static readonly CanDeleteStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.DeleteStudies)
    .Build();

  /**
   * Requires a user is Authenticated and can Remove Study Users
   */
  public static readonly CanRemoveStudyUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.RemoveStudyUsers)
    .Build();

  /**
   * Requires a user is Authenticated and can View Users
   */
  public static readonly CanViewUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.ViewUsers)
    .Build();

  /**
   * Requires a user is Authenticated and can Edit Configuration
   */
  public static readonly CanEditConfig = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.EditConfig)
    .Build();
}
