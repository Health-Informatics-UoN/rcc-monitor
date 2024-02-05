export const Permissions = {
  ViewSiteReports: "ViewSiteReports",
  GenerateSyntheticData: "GenerateSyntheticData",
  ViewStudies: "ViewStudies",
  DeleteStudies: "DeleteStudies",
  UpdateStudies: "UpdateStudies",
  RemoveStudyUsers: "RemoveStudyUsers",
  ViewUsers: "ViewUsers",
  EditConfig: "EditConfig",
} as const;
// TODO: this type should live in lib/auth - but be satisfied OR ? extended here.
export type Permission = keyof typeof Permissions;
