import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { pathToRegexp } from "path-to-regexp";

import { PathAuthorizationMapping } from "@/lib/auth/types";

/**
 * Checks if the user is authorised to access the request path.
 * @param req: Next request
 * @param token: JWT token
 * @returns true if user is authorised.
 */
export const isAuthorized = ({
  req,
  token,
  pathAuthMapping,
}: {
  req: NextRequest;
  token: JWT | null;
  pathAuthMapping: PathAuthorizationMapping;
}): boolean => {
  const currentPath = req.nextUrl.pathname;
  const filteredPolicies = Object.entries(pathAuthMapping)
    .filter(([path]) => pathToRegexp(path).test(currentPath))
    .map(([_, policy]) => policy);

  // If there are no policies to check then user is authorised.
  if (filteredPolicies.length === 0) return true;

  // Check if the user is authorized based on the filtered policies
  return filteredPolicies.some((policy) => policy.isAuthorized(token));
};
