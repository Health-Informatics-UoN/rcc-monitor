import { JWT } from "next-auth/jwt";

import { permissions } from "@/auth/permissions";

type AuthorizationPolicy = {
  isAuthorized: (token: JWT | null) => boolean;
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
  RequireRoles(
    ...roles: (keyof typeof permissions)[]
  ): AuthorizationPolicyBuilder {
    this.requirements.push({
      isAuthorized: (token: JWT | null) =>
        roles.every((role) => token?.permissions.includes(role)),
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
      isAuthorized: (token: JWT | null) =>
        this.requirements.every((policy) => policy.isAuthorized(token)),
      getPermissions: () =>
        this.requirements.flatMap((policy) => policy.getPermissions()),
    };
    return combinedPolicy;
  }
}
