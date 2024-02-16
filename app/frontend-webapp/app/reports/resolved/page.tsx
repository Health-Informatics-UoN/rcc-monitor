import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  triggerStyle,
} from "@/components/shadow-ui/Tabs";
import { getReports } from "@/api/reports";
import { css } from "@/styled-system/css";
import { flex } from "@/styled-system/patterns";
import { h1 } from "@/styled-system/recipes";
import { Alert } from "@/components/core/Alert";
import { DataTable } from "@/components/data-table";
import { columns, parentSiteIdConflictColumns } from "./columns";

export default async function ResolvedReportsPage() {
  const reports = await getReports();
  const resolved = reports.filter((x) => x.status.name === "Resolved");

  const resolvedReports = [
    {
      title: "Site Conflicts",
      reports: resolved.filter((x) => x.reportType.name === "ConflictingSite"),
    },
    {
      title: "Site Name Conflicts",
      reports: resolved.filter(
        (x) => x.reportType.name === "ConflictingSiteName"
      ),
    },
    {
      title: "Site Parent Id Conflicts",
      reports: resolved.filter(
        (x) => x.reportType.name === "ConflictingSiteParent"
      ),
    },
  ];

  return (
    <>
      <h1 className={h1()}>Resolved Reports</h1>
      <Tabs
        defaultValue="Site Conflicts"
        w="95%"
        position="relative"
        m="30px auto"
      >
        <TabsList
          w="full"
          justifyContent="flex-start"
          rounded="0"
          borderBottom="base"
          bg="transparent"
          p="0"
        >
          {resolvedReports.map((report, index) => (
            <TabsTrigger
              key={index}
              className={triggerStyle}
              value={report.title}
            >
              {report.title}
            </TabsTrigger>
          ))}
        </TabsList>
        {resolvedReports.map((report, index) => (
          <TabsContent key={index} value={report.title}>
            {report.reports.length <= 0 ? (
              <Alert
                css={{ m: "50px auto" }}
                message="There are currently no reports."
              />
            ) : (
              <>
                <p className={css({ m: "30px 0px" })}>
                  These site conflicts have been resolved
                </p>
                <div className={flex({ gap: "6", direction: "column" })}>
                  <DataTable
                    columns={
                      report.title === "Site Parent Id Conflicts"
                        ? parentSiteIdConflictColumns
                        : columns
                    }
                    data={report.reports}
                  />
                </div>
              </>
            )}
          </TabsContent>
        ))}
      </Tabs>
    </>
  );
}
