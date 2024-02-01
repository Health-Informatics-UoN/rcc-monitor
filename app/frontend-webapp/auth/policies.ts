import { AuthorizationPolicyBuilder } from "@/auth/AuthorizationPolicyBuilder";
import { permissions } from "@/auth/permissions";

export class AuthorizationPolicies {
  // TODO: check if the user has a token at all
  public static readonly IsAuthenticatedUser =
    new AuthorizationPolicyBuilder().Build();

  public static readonly CanViewSiteReports = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRoles(permissions.ViewSiteReports)
    .Build();

  public static readonly CanGenerateSyntheticData =
    new AuthorizationPolicyBuilder()
      .Combine(AuthorizationPolicies.IsAuthenticatedUser)
      .RequireRoles(permissions.GenerateSyntheticData)
      .Build();
}
