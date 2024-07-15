import { Activity, FileSpreadsheet, MonitorStop } from "lucide-react";
import { getServerSession } from "next-auth";

import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import ActionCard from "@/components/core/ActionCard";
import { isUserAuthorized, options } from "@/lib/auth";

export default async function UserHome() {
  const session = await getServerSession(options);

  return (
    <>
      <h2 className="m-[10px] p-[10px] text-[30px] font-medium">Welcome.</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-[15px]">
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
