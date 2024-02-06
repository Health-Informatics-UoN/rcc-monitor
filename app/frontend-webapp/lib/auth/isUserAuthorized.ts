import { JWT } from "next-auth/jwt";

import { AuthorizationPolicy } from "@/lib/auth";

/**
 * Checks if a token meets a specific authorization policy.
 * @param token token to check
 * @param policy policy to check
 * @returns true if the token meets the authorization policy.
 */
export function isUserAuthorized(
  token: JWT,
  policy: AuthorizationPolicy
): boolean {
  return policy.isAuthorized(token);
}
