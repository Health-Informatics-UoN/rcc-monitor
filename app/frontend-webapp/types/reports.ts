export type ReportType = {
  id: number;
  name: "ConflictingSite" | "ConflictingSiteName" | "ConflictingSiteParent";
};

export type Status = {
  id: number;
  name: "Resolved" | "Viewed" | "Active";
};

export type Instance = {
  id: number;
  name: string;
};

export type Site = {
  siteName: string;
  siteId: string;
  instance: string;
  parentSiteId: string;
};

export type ReportModel = {
  id: number;
  dateTime: Date;
  lastChecked: Date;
  description: string;
  reportType: ReportType;
  status: Status;
  sites: Site[];
};

export type SiteReport = {
  dateOccured: string;
  lastChecked: string;
  siteId?: string;
  siteName?: string;
  siteNameInBuild?: string;
  siteNameInProd?: string;
  siteNameInUAT?: string;
  parentSiteIdInBuild?: string;
  parentSiteIdInProd?: string;
  parentSiteIdInUAT?: string;
};
