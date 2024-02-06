import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { withAuth } from "next-auth/middleware";

import { policyPathMapping } from "@/auth/PolicyPathMapping";
import { isAuthorized } from "@/lib/middlewares/isAuthorized";

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
      return isAuthorized({
        req,
        token,
        policyPathMapping,
      });
    },
  },
});

// We can't move this to /lib/auth until it supports spread operators
// https://nextjs.org/docs/messages/invalid-page-config
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
