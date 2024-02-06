import { NextRequest } from "next/server";
import { JWT } from "next-auth/jwt";

import { PathPolicyMapping } from "@/lib/auth/types";

/**
 * Checks if the user is authorised to access the request path.
 * @param req: Next request
 * @param token: JWT token
 * @returns true if user is authorised.
 */
export const isAuthorized = ({
  req,
  token,
  policyPathMapping,
}: {
  req: NextRequest;
  token: JWT | null;
  policyPathMapping: PathPolicyMapping;
}): boolean => {
  const currentPath = req.nextUrl.pathname;
  const policy = policyPathMapping[currentPath];

  // If there are no policies to check then user is authorised.
  if (!policy) return true;

  // Check if the user is authorized based on the filtered policies
  return policy.isAuthorized(token);
};