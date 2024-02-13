import { withAuth } from "next-auth/middleware";

import { routeAuthMapping } from "@/auth/RouteAuthMapping";
import { getAuthorized } from "@/lib/auth";

export default withAuth({
  pages: {
    signIn: "/signIn",
  },
  callbacks: {
    authorized: getAuthorized(routeAuthMapping),
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
