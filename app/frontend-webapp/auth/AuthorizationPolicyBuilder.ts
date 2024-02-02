type AuthorizationPolicy = {
  isAuthorized: (userPermissions: string[]) => boolean;
  getPermissions: () => string[];
};

/**
 * Used for building Authorization policies.
 */
export class AuthorizationPolicyBuilder {
  /**
   * List of policies that must succeed for this policy to be succesful.
   */
  private requirements: AuthorizationPolicy[] = [];

  /**
   * Combines the specificed policy into the current instance.
   * @param policies The policy to combine
   * @returns A reference to the instance.
   */
  Combine(policy: AuthorizationPolicy): AuthorizationPolicyBuilder {
    this.requirements.push(policy);
    return this;
  }

  /**
   * Adds role requirement to the builder.
   * @param roles The roles required.
   * @returns A reference to the instance.
   */
  RequireRoles(...roles: string[]): AuthorizationPolicyBuilder {
    this.requirements.push({
      isAuthorized: (userPermissions: string[]) =>
        roles.every((role) => userPermissions?.includes(role)),
      getPermissions: () => roles,
    });
    return this;
  }

  /**
   * Adds a requirement to the current instance to enforce that the current user is authenticated.
   * // TODO: Implement this logic
   * @returns A reference to the instance.
   */
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
        this.requirements.every((policy) =>
          policy.isAuthorized(userPermissions)
        ),
      getPermissions: () =>
        this.requirements.flatMap((policy) => policy.getPermissions()),
    };
    return combinedPolicy;
  }
}
