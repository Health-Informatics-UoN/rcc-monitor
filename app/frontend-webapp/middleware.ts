import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";
import { withAuth } from "next-auth/middleware";

import { policyPathMapping } from "@/auth/PolicyPathMapping";
import { isAuthorized } from "@/lib/middlewares/auth";
import { defaultConfig } from "@/lib/middlewares/defaultConfig";

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

export const config = {
  defaultConfig,
};
