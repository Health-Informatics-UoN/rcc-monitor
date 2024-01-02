import { SidebarItem } from "@/types";

export const SidebarItems: SidebarItem[] = [
  {
    name: "Studies",
    path: "/studies",
    permission: "ViewStudies",
    icon: "Activity",
    featureFlag: "studyManagementEnabled",
  },
  {
    name: "Site Reports",
    path: "/reports",
    permission: "ViewSiteReports",
    icon: "MonitorStop",
    featureFlag: "siteMonitoringEnabled",
  },
  {
    name: "Synthetic Data",
    path: "/synthetic-data",
    permission: "GenerateSyntheticData",
    icon: "FileSpreadsheet",
    featureFlag: "syntheticDataEnabled",
  },
  {
    name: "Settings",
    path: "/settings",
    permission: "EditConfig",
    icon: "Settings",
  },
];
