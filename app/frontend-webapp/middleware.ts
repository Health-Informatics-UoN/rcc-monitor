import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { withAuth } from "next-auth/middleware";
import { pathToRegexp } from "path-to-regexp";

import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import { permissions } from "@/auth/permissions";

// Map the permission that a path needs to be accessed.
const permissionPathMapping = {
  [permissions.ViewSiteReports]: ["/reports", "/reports/resolved"],
  [permissions.GenerateSyntheticData]: ["/synthetic-data"],
  [permissions.ViewStudies]: ["/studies", "/studies/:id"],
  [permissions.UpdateStudies]: ["/studies/:id/edit"],
  [permissions.EditConfig]: ["/settings"],
};

// List of policies to check
// TODO: probably move this into the auth section somewhere
// as we're effectively "registering" policies here.
const policiesToCheck = [
  AuthorizationPolicies.CanViewSiteReports,
  AuthorizationPolicies.CanGenerateSyntheticData,
  AuthorizationPolicies.CanViewStudies,
  AuthorizationPolicies.CanDeleteStudies,
  AuthorizationPolicies.CanRemoveStudyUsers,
  AuthorizationPolicies.CanViewUsers,
  AuthorizationPolicies.CanEditConfig,
];

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

      const currentPath = req.nextUrl.pathname;

      // need to filter the policies to check based on the paths... ?

      const isAuthorized = policiesToCheck.some((policy) =>
        policy.isAuthorized(userPermissions)
      );

      for (const permission in permissionPathMapping) {
        for (const mappedPath of permissionPathMapping[permission]) {
          const regex = pathToRegexp(mappedPath);
          if (
            userPermissions?.includes(permission) &&
            regex.exec(currentPath)
          ) {
            return true; // Authorized for this permission and path
          }
        }
      }

      // Authorized for non-mapped paths
      if (
        !Object.values(permissionPathMapping).some((paths) =>
          paths.includes(currentPath)
        )
      ) {
        return true;
      }

      // Not authorised if they dont have the permission.
      return false;
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
