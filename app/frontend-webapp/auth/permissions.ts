/**
 * Checks a list of permission contains a specific permission.
 * @param permissions list of permissions to check
 * @param permissionToCheck permission to check
 * @returns true if the permission exists in the list
 */
export function isUserAuthorized(
  permissions: string[] | undefined,
  permissionToCheck: string
): boolean {
  return permissions?.includes(permissionToCheck) ?? false;
}

export const permissions = {
  ViewSiteReports: "ViewSiteReports",
  GenerateSyntheticData: "GenerateSyntheticData",
  ViewStudies: "ViewStudies",
  DeleteStudies: "DeleteStudies",
  UpdateStudies: "UpdateStudies",
  RemoveStudyUsers: "RemoveStudyUsers",
  ViewUsers: "ViewUsers",
  EditConfig: "EditConfig",
} as const;

export type Permission = keyof typeof permissions;
