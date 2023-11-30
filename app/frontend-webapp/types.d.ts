type ReportType = {
  id: number;
  name: "ConflictingSite" | "ConflictingSiteName" | "ConflictingSiteParent";
};

type Status = {
  id: number;
  name: "Resolved" | "Viewed" | "Active";
};

type Instance = {
  id: number;
  name: string;
};

type Site = {
  siteName: string;
  siteId: string;
  instance: string;
  parentSiteId: string;
};

type ReportModel = {
  id: number;
  dateTime: Date;
  description: string;
  reportType: ReportType;
  status: Status;
  sites: Site[];
};
