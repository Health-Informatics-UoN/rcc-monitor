import { assertPermission } from "@/lib/auth";

/**
 * Custom permissions from Keycloak.
 */
export class Permissions {
  public static readonly ViewSiteReports = assertPermission("ViewSiteReports");
  public static readonly GenerateSyntheticData = assertPermission(
    "GenerateSyntheticData"
  );
  public static readonly ViewStudies = assertPermission("ViewStudies");
  public static readonly DeleteStudies = assertPermission("DeleteStudies");
  public static readonly UpdateStudies = assertPermission("UpdateStudies");
  public static readonly RemoveStudyUsers =
    assertPermission("RemoveStudyUsers");
  public static readonly ViewUsers = assertPermission("ViewUsers");
  public static readonly EditConfig = assertPermission("EditConfig");
}
