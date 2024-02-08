import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import { SidebarItem } from "@/types";

export const SidebarItems: SidebarItem[] = [
  {
    name: "Studies",
    path: "/studies",
    policy: AuthorizationPolicies.CanViewStudies,
    icon: "Activity",
    featureFlag: "studyManagementEnabled",
  },
  {
    name: "Site Reports",
    path: "/reports",
    policy: AuthorizationPolicies.CanViewSiteReports,
    icon: "MonitorStop",
    featureFlag: "siteMonitoringEnabled",
  },
  {
    name: "Synthetic Data",
    path: "/synthetic-data",
    policy: AuthorizationPolicies.CanGenerateSyntheticData,
    icon: "FileSpreadsheet",
    featureFlag: "syntheticDataEnabled",
  },
  {
    name: "Settings",
    path: "/settings",
    policy: AuthorizationPolicies.CanEditConfig,
    icon: "Settings",
  },
];
