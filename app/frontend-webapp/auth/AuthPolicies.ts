import { AuthorizationPolicyBuilder } from "@/auth/AuthorizationPolicyBuilder";
import { permissions } from "@/auth/permissions";

// TODO: Add documentation against these like the backend.
export class AuthorizationPolicies {
  public static readonly IsAuthenticatedUser = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

  public static readonly CanViewSiteReports = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(permissions.ViewSiteReports)
    .Build();

  public static readonly CanGenerateSyntheticData =
    new AuthorizationPolicyBuilder()
      .Combine(AuthorizationPolicies.IsAuthenticatedUser)
      .RequirePermissions(permissions.GenerateSyntheticData)
      .Build();

  public static readonly CanViewStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(permissions.ViewStudies)
    .Build();

  public static readonly CanDeleteStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(permissions.DeleteStudies)
    .Build();

  public static readonly CanRemoveStudyUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(permissions.RemoveStudyUsers)
    .Build();

  public static readonly CanViewUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(permissions.ViewUsers)
    .Build();

  public static readonly CanEditConfig = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(permissions.EditConfig)
    .Build();
}
