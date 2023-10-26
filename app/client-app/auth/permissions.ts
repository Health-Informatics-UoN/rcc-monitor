/**
 * Checks a list of permission contains a specific permission.
 * @param permissions list of permissions to check
 * @param permissionToCheck permission to check
 * @returns true if the permission exists in the list
 */
export function hasPermission(
  permissions: string[] | undefined,
  permissionToCheck: string
): boolean {
  return permissions?.includes(permissionToCheck) ?? false;
}

export const permissions = {
  ViewSiteReports: "ViewSiteReports",
  GenerateSyntheticData: "GenerateSyntheticData",
  ViewStudies: "ViewStudies",
};
