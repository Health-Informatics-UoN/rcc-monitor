import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import { RouteAuthorizationMapping } from "@/lib/auth";

// Map the route and its required policy that needs to be authenticated.
export const routeAuthMapping: RouteAuthorizationMapping = {
  "/reports": AuthorizationPolicies.CanViewSiteReports,
  "/reports/resolved": AuthorizationPolicies.CanViewSiteReports,
  "/synthetic-data": AuthorizationPolicies.CanGenerateSyntheticData,
  "/studies": AuthorizationPolicies.CanViewStudies,
  "/studies/:id": AuthorizationPolicies.CanViewStudies,
  "/settings": AuthorizationPolicies.CanEditConfig,
};
