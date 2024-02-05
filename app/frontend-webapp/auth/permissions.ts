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
