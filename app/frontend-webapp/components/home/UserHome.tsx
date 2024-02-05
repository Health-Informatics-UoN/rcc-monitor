import { css } from "@/styled-system/css";
import ActionCard from "@/components/ActionCard";
import { FileSpreadsheet, MonitorStop, Activity } from "lucide-react";
import { isUserAuthorized, permissions } from "@/auth/permissions";
import { getServerSession } from "next-auth";
import { options } from "@/auth/AuthOptions";
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
        {isUserAuthorized(
          session?.permissions,
          permissions.ViewSiteReports
        ) && (
          <ActionCard
            to="/reports"
            title="Site Monitoring"
            icon={<MonitorStop />}
            description="View site data and information"
          />
        )}

        {isUserAuthorized(
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

        {isUserAuthorized(session?.permissions, permissions.ViewStudies) && (
          <ActionCard
            to="/studies"
            title="RedCap Studies"
            icon={<Activity />}
            description="View study information and alerts"
          />
        )}
      </div>
    </>
  );
}
