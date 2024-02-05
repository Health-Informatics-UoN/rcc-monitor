/**
 * TODO: Make this token / policy based.
 * Checks if a token meets a specific authorization policy.
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
