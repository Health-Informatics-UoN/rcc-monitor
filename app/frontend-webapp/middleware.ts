import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { withAuth } from "next-auth/middleware";
import { pathToRegexp } from "path-to-regexp";

import { AuthorizationPolicies } from "@/auth/AuthPolicies";

// Map the policy that a path needs to be accessed.
const policyPathMapping = {
  "/reports": AuthorizationPolicies.CanViewSiteReports,
  "/reports/resolved": AuthorizationPolicies.CanViewSiteReports,
  "/synthetic-data": AuthorizationPolicies.CanGenerateSyntheticData,
  "/studies": AuthorizationPolicies.CanViewStudies,
  "/studies:id/": AuthorizationPolicies.CanViewStudies,
  "/settings": AuthorizationPolicies.CanEditConfig,
};

export default withAuth({
  callbacks: {
    authorized: ({ req, token }: { req: NextRequest; token: JWT | null }) => {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const userPermissions: any = token?.permissions;

      // Filter the policies based on the current path
      const currentPath = req.nextUrl.pathname;
      const filteredPolicies = Object.entries(policyPathMapping).filter(
        ([path]) => pathToRegexp(path).test(currentPath)
      );
      const policiesToCheck = filteredPolicies.map(([_, policy]) => policy);

      // Check if the user is authorized based on the filtered policies
      const isAuthorized =
        policiesToCheck.length === 0 ||
        policiesToCheck.some((policy) => policy.isAuthorized(userPermissions));

      if (isAuthorized) {
        console.log("Authorised");
        return true;
      } else {
        console.log("Non authorised.");
        return false;
      }
    },
  },
});

export const config = {
  matcher: [
    /*
     * Match all paths except for:
     * 1. / index page
     * 2. /api routes
     * 3. /_next (Next.js internals)
     * 4. /_static (inside /public)
     * 5. all root files inside /public (e.g. /favicon.ico)
     */
    "/((?!$|api/|_next/|_static/|[\\w-]+\\.\\w+).*)",
  ],
};
