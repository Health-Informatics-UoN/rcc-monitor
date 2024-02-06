import { Activity, FileSpreadsheet, MonitorStop } from "lucide-react";
import { getServerSession } from "next-auth";

import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import ActionCard from "@/components/ActionCard";
import { options } from "@/lib/auth";
import { isUserAuthorized } from "@/lib/auth";
import { css } from "@/styled-system/css";
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
          session?.token,
          AuthorizationPolicies.CanViewSiteReports
        ) && (
          <ActionCard
            to="/reports"
            title="Site Monitoring"
            icon={<MonitorStop />}
            description="View site data and information"
          />
        )}

        {isUserAuthorized(
          session?.token,
          AuthorizationPolicies.CanGenerateSyntheticData
        ) && (
          <ActionCard
            to="/synthetic-data"
            title="Synthetic Data Generation"
            icon={<FileSpreadsheet />}
            description="Generate synthetic data"
          />
        )}

        {isUserAuthorized(
          session?.token,
          AuthorizationPolicies.CanViewStudies
        ) && (
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
