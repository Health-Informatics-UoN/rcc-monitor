import { BarChart2, ClipboardCheckIcon, Construction } from "lucide-react";
import { Activity, FileSpreadsheet, MonitorStop, Settings } from "lucide-react";

export type FacetsFilter = {
  value: string;
  label: string;
  icon: keyof typeof Icons;
};

/**
 * Typed icons to pass from the server to client.
 */
export const Icons = {
  Construction: Construction,
  BarChart2: BarChart2,
  Clipboard: ClipboardCheckIcon,
  Activity: Activity,
  MonitorStop: MonitorStop,
  FileSpreadsheet: FileSpreadsheet,
  Settings: Settings,
};
