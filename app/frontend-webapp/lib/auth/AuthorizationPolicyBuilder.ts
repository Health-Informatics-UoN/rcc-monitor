import { JWT } from "next-auth/jwt";

import { AuthorizationPolicy, Permission } from "@/lib/auth/types";

export function assertPermission(permission: string): Permission {
  return permission as Permission;
}

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
   * @param policy The policy to combine
   * @returns A reference to the instance.
   */
  Combine(policy: AuthorizationPolicy): AuthorizationPolicyBuilder {
    this.requirements.push(policy);
    return this;
  }

  /**
   * Adds policy requirement to the builder.
   * @param permissions The permissions required.
   * @returns A reference to the instance.
   */
  RequirePermissions(...permissions: Permission[]): AuthorizationPolicyBuilder {
    this.requirements.push({
      isAuthorized: (token: JWT | null) =>
        permissions.every((policy) => token?.permissions.includes(policy)),
      getPermissions: () => permissions,
    });
    return this;
  }

  /**
   * Adds a requirement to the current instance to enforce that the current user is authenticated.
   * A user is authenticated if they have a token.
   * @returns A reference to the instance.
   */
  RequireAuthenticatedUser(): AuthorizationPolicyBuilder {
    this.requirements.push({
      isAuthorized: (token: JWT | null) => token !== null,
      getPermissions: () => [],
    });
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
