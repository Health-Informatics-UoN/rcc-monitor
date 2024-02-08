import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { pathToRegexp } from "path-to-regexp";

import { RouteAuthorizationMapping } from "@/lib/auth/types";

/**
 * Checks if the user is authorised to access the request path.
 * @param routeAuthMapping: Route mapping to authorize by.
 * @returns true if user is authorised.
 */
export const getAuthorized =
  (routeAuthMapping: RouteAuthorizationMapping) =>
  ({ req, token }: { req: NextRequest; token: JWT | null }): boolean => {
    const currentPath = req.nextUrl.pathname;
    const filteredPolicies = Object.entries(routeAuthMapping)
      .filter(([path]) => pathToRegexp(path).test(currentPath))
      .map(([_, policy]) => policy);

    // If there are no policies to check then user is authorised.
    if (filteredPolicies.length === 0) return true;

    // Check if the user is authorized based on the filtered policies
    return filteredPolicies.some((policy) => policy.isAuthorized(token));
  };
