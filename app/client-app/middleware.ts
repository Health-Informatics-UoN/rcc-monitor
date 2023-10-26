import { withAuth } from "next-auth/middleware";
import { JWT } from "next-auth/jwt";
import { NextRequest } from "next/server";
import { permissions } from "@/auth/permissions";

// Map the permission that a path needs to be accessed.
const permissionPathMapping = {
  [permissions.ViewSiteReports]: ["/reports"],
  [permissions.GenerateSyntheticData]: ["/synthetic-data"],
  [permissions.ViewStudies]: ["/studies"],
};

export default withAuth({
  callbacks: {
    authorized: ({ req, token }: { req: NextRequest; token: JWT | null }) => {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const userPermissions: any = token?.permissions;
      if (!userPermissions) {
        return false; // Not authorized if no permissions exist.
      }

      const currentPath = req.nextUrl.pathname;

      for (const permission in permissionPathMapping) {
        if (
          userPermissions.includes(permission) &&
          permissionPathMapping[permission].includes(currentPath)
        ) {
          return true; // Authorized for this permission and path
        }
      }

      return false; // Not authorized for this path and permission
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
