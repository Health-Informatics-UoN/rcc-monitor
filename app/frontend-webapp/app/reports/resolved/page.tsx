import { AlertCircle } from "lucide-react";

import { getReports } from "@/api/reports";
import { DataTable } from "@/components/data-table";
import { Alert, AlertTitle } from "@/components/ui/alert";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  triggerStyle,
} from "@/components/ui/tabs";

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
      <h1 className={`h1`}>Resolved Reports</h1>
      <Tabs
        defaultValue="Site Conflicts"
        className="w-[95%] relative my-[30px] mx-auto"
      >
        <TabsList className="w-full justify-start rounded-none border-b bg-transparent p-0">
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
              <Alert className="bg-red-200 my-12 mx-0">
                <AlertCircle className="icon-lg" />
                <AlertTitle>There are currently no reports.</AlertTitle>
              </Alert>
            ) : (
              <>
                <p className="my-7 mx-0">
                  These site conflicts have been resolved
                </p>
                <div className="flex flex-col gap-6">
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
