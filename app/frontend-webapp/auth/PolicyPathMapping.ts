import { AuthorizationPolicies } from "@/auth/AuthPolicies";

// Map the path and its required policy that needs to be authenticated.
// TODO: This should probably be a type/interface that is satisfied.
export const policyPathMapping = {
  "/reports": AuthorizationPolicies.CanViewSiteReports,
  "/reports/resolved": AuthorizationPolicies.CanViewSiteReports,
  "/synthetic-data": AuthorizationPolicies.CanGenerateSyntheticData,
  "/studies": AuthorizationPolicies.CanViewStudies,
  "/studies:id": AuthorizationPolicies.CanViewStudies,
  "/settings": AuthorizationPolicies.CanEditConfig,
};
