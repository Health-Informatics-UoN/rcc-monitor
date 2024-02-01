import { AuthorizationPolicyBuilder } from "@/auth/AuthorizationPolicyBuilder";
import { permissions } from "@/auth/permissions";

export class AuthorizationPolicies {
  public static readonly IsAuthenticatedUser = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

  public static readonly CanViewSiteReports = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.ViewSiteReports)
    .Build();

  public static readonly CanGenerateSyntheticData =
    new AuthorizationPolicyBuilder()
      .Combine(AuthorizationPolicies.IsAuthenticatedUser)
      .RequireRoles(permissions.GenerateSyntheticData)
      .Build();

  public static readonly CanViewStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.ViewStudies)
    .Build();

  public static readonly CanDeleteStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.DeleteStudies)
    .Build();

  public static readonly CanRemoveStudyUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.RemoveStudyUsers)
    .Build();

  public static readonly CanViewUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.ViewUsers)
    .Build();

  public static readonly CanEditConfig = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.EditConfig)
    .Build();
}
