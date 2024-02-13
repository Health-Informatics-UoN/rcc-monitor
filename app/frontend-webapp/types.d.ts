import { Icons } from "@/components/shared/Icons";
import { AuthorizationPolicy } from "@/lib/auth";
import { FeatureFlagModel } from "@/types/config";

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
  lastChecked: Date;
  description: string;
  reportType: ReportType;
  status: Status;
  sites: Site[];
};

type SiteReport = {
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

interface SidebarLink {
  name: string;
  path?: string;
  icon?: keyof typeof Icons;
}

interface SidebarItem extends SidebarLink {
  policy?: AuthorizationPolicy;
  featureFlag?: keyof FeatureFlagModel;
  children?: SideBarLink[];
}
