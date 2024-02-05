import { Permissions } from "@/auth/Permissions";
import { AuthorizationPolicyBuilder } from "@/lib/auth";

// TODO: Add documentation against these like the backend.
export class AuthorizationPolicies {
  public static readonly IsAuthenticatedUser = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

  public static readonly CanViewSiteReports = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.ViewSiteReports)
    .Build();

  public static readonly CanGenerateSyntheticData =
    new AuthorizationPolicyBuilder()
      .Combine(AuthorizationPolicies.IsAuthenticatedUser)
      .RequirePermissions(Permissions.GenerateSyntheticData)
      .Build();

  public static readonly CanViewStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.ViewStudies)
    .Build();

  public static readonly CanDeleteStudies = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.DeleteStudies)
    .Build();

  public static readonly CanRemoveStudyUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.RemoveStudyUsers)
    .Build();

  public static readonly CanViewUsers = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.ViewUsers)
    .Build();

  public static readonly CanEditConfig = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequirePermissions(Permissions.EditConfig)
    .Build();
}
