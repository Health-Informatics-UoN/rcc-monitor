import { css } from "@/styled-system/css";
import ActionCard from "@/components/ActionCard";
import {
  AlertOctagon,
  FileSpreadsheet,
  FolderPlus,
  MonitorStop,
} from "lucide-react";
import { hasPermission, permissions } from "@/auth/permissions";
import { getServerSession } from "next-auth";
import { options } from "@/auth/options";
import { grid } from "@/styled-system/patterns";

export default async function UserHome() {
  const session = await getServerSession(options);

  return (
    <>
      <h2
        className={css({
          m: "10px",
          p: "10px",
          fontSize: "30px",
          fontWeight: "500",
        })}
      >
        Welcome.
      </h2>
      <div
        className={grid({
          columns: { base: 1, md: 2, lg: 3 },
          gap: "15px",
        })}
      >
        {hasPermission(session?.permissions, permissions.ViewSiteReports) && (
          <ActionCard
            to="/reports"
            title="Site Monitoring"
            icon={<MonitorStop />}
            description="View site data and information"
          />
        )}

        {hasPermission(
          session?.permissions,
          permissions.GenerateSyntheticData
        ) && (
          <ActionCard
            to="/synthetic-data"
            title="Synthetic Data Generation"
            icon={<FileSpreadsheet />}
            description="Generate synthetic data"
          />
        )}

        {hasPermission(session?.permissions, permissions.ViewStudies) && (
          <ActionCard
            to="/studies"
            title="Register Study"
            icon={<FolderPlus />}
            description="Register a study"
          />
        )}

        {/* Todo: Add condition to render on Pemission */}
        <ActionCard
          to="/randomization-alerts"
          title="Randomization Alerts"
          icon={<AlertOctagon />}
          description="Manage randomisation alerts"
        />
      </div>
    </>
  );
}
