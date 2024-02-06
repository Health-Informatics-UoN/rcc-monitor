import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import { PathAuthorizationMapping } from "@/lib/auth";

// Map the path and its required policy that needs to be authenticated.
export const pathAuthMapping: PathAuthorizationMapping = {
  "/reports": AuthorizationPolicies.CanViewSiteReports,
  "/reports/resolved": AuthorizationPolicies.CanViewSiteReports,
  "/synthetic-data": AuthorizationPolicies.CanGenerateSyntheticData,
  "/studies": AuthorizationPolicies.CanViewStudies,
  "/studies/:id": AuthorizationPolicies.CanViewStudies,
  "/settings": AuthorizationPolicies.CanEditConfig,
};
