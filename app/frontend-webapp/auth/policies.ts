import { permissions } from "@/auth/permissions";

type AuthorizationPolicy = {
  isAuthorized: (userPermissions: string[]) => boolean;
  getPermissions: () => string[];
};

class AuthorizationPolicyBuilder {
  private policies: AuthorizationPolicy[] = [];

  Combine(...policies: AuthorizationPolicy[]): AuthorizationPolicyBuilder {
    this.policies.push(...policies);
    return this;
  }

  RequireRealmRoles(...roles: string[]): AuthorizationPolicyBuilder {
    this.policies.push({
      isAuthorized: (userPermissions: string[]) =>
        roles.every((role) => userPermissions.includes(role)),
      getPermissions: () => roles,
    });
    return this;
  }

  Build(): AuthorizationPolicy {
    const combinedPolicy: AuthorizationPolicy = {
      isAuthorized: (userPermissions: string[]) =>
        this.policies.every((policy) => policy.isAuthorized(userPermissions)),
      getPermissions: () =>
        this.policies.flatMap((policy) => policy.getPermissions()),
    };
    return combinedPolicy;
  }
}

export class AuthorizationPolicies {
  // TODO: check if the user has a token at all
  public static readonly IsAuthenticatedUser =
    new AuthorizationPolicyBuilder().Build();

  public static readonly CanViewSiteReports = new AuthorizationPolicyBuilder()
    .Combine(AuthorizationPolicies.IsAuthenticatedUser)
    .RequireRealmRoles(permissions.ViewSiteReports)
    .Build();

  public static readonly CanGenerateSyntheticData =
    new AuthorizationPolicyBuilder()
      .Combine(AuthorizationPolicies.IsAuthenticatedUser)
      .RequireRealmRoles(permissions.GenerateSyntheticData)
      .Build();
}
