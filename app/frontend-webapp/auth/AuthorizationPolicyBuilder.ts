type AuthorizationPolicy = {
  isAuthorized: (userPermissions: string[]) => boolean;
  getPermissions: () => string[];
};

/**
 * Based on the .NET Core Auth policy builder.
 */
export class AuthorizationPolicyBuilder {
  private policies: AuthorizationPolicy[] = [];

  /**
   * Combines the specificed policy into the current instance.
   * @param policies The policy to combine
   * @returns A reference to the instance.
   */
  Combine(policy: AuthorizationPolicy): AuthorizationPolicyBuilder {
    this.policies.push(policy);
    return this;
  }

  /**
   * Adds role requirement to the builder.
   * @param roles The roles required.
   * @returns A reference to the instance.
   */
  RequireRoles(...roles: string[]): AuthorizationPolicyBuilder {
    this.policies.push({
      isAuthorized: (userPermissions: string[]) =>
        roles.every((role) => userPermissions.includes(role)),
      getPermissions: () => roles,
    });
    return this;
  }

  RequireAuthenticatedUser(): AuthorizationPolicyBuilder {
    return this;
  }

  /**
   * Builds a new AuthorizationPolicy from the requirements.
   * @returns A new AuthorizationPolicy built from the requirements.
   */
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
