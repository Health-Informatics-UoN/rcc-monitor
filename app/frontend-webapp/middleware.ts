import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { withAuth } from "next-auth/middleware";
import { pathToRegexp } from "path-to-regexp";

import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import { defaultConfig } from "@/lib/middlewares/defaultConfig";

// Map the path and its required policy that needs to be authenticated.
const policyPathMapping = {
  "/reports": AuthorizationPolicies.CanViewSiteReports,
  "/reports/resolved": AuthorizationPolicies.CanViewSiteReports,
  "/synthetic-data": AuthorizationPolicies.CanGenerateSyntheticData,
  "/studies": AuthorizationPolicies.CanViewStudies,
  "/studies:id": AuthorizationPolicies.CanViewStudies,
  "/settings": AuthorizationPolicies.CanEditConfig,
};

export default withAuth({
  pages: {
    signIn: "/signIn",
  },
  callbacks: {
    /**
     * Checks if the user is authorised to access the request path.
     * @param req: Next request
     * @param token: JWT token
     * @returns true if user is authorised.
     */
    authorized: ({
      req,
      token,
    }: {
      req: NextRequest;
      token: JWT | null;
    }): boolean => {
      // Filter the policies based on the current path
      const currentPath = req.nextUrl.pathname;
      const filteredPolicies = Object.entries(policyPathMapping)
        .filter(([path]) => pathToRegexp(path).test(currentPath))
        .map(([_, policy]) => policy);

      // If there are no policies to check then user is authorised.
      if (filteredPolicies.length === 0) return true;

      // Check if the user is authorized based on the filtered policies
      return filteredPolicies.some((policy) => policy.isAuthorized(token));
    },
  },
});

export const config = {
  defaultConfig,
};
