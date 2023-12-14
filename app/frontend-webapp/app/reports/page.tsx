import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  triggerStyle,
} from "@/components/shadow-ui/Tabs";
import { Alert } from "@/components/Alert";
import { getReports } from "@/api/reports";
import { Metadata } from "next";
import { flex, vstack } from "@/styled-system/patterns";
import { css } from "@/styled-system/css";
import { format } from "date-fns";
import { h1 } from "@/styled-system/recipes";
import { DataTable } from "@/components/data-table";
import {
  siteConflictsColumns,
  siteNameConflictsColumns,
  siteParentIdConflictsColumns,
} from "./columns";

export const metadata: Metadata = {
  title: "RedCap Site Reports",
};

export default async function Reports() {
  const reports = await getReports();

  const siteConflictsReports: SiteReport[] = reports
    .filter((v) => v.reportType.name == "ConflictingSite")
    .flatMap((report) =>
      report.sites.map((site) => ({
        dateOccured: format(new Date(report.dateTime), "dd-MM-yyyy"),
        lastChecked: format(new Date(report.lastChecked), "dd-MM-yyyy hh:mm"),
        siteId: site.siteId,
        siteName: site.siteName,
      }))
    );

  const siteNameConflictsReports: SiteReport[] = reports
    .filter((v) => v.reportType.name == "ConflictingSiteName")
    .flatMap((report) =>
      report.sites.map((site) => {
        return {
          dateOccured: format(new Date(report.dateTime), "dd-MM-yyyy"),
          lastChecked: format(new Date(report.lastChecked), "dd-MM-yyyy hh:mm"),
          siteId: site.siteId,
          siteNameInBuild: site.instance === "Build" ? site.siteName : "",
          siteNameInProd: site.instance === "Production" ? site.siteName : "",
          siteNameInUAT: site.instance === "UAT" ? site.siteName : "",
        };
      })
    );

  const siteParentIdConflictsReports: SiteReport[] = reports
    .filter((v) => v.reportType.name == "ConflictingSiteParent")
    .flatMap((report) =>
      report.sites.map((site) => ({
        dateOccured: format(new Date(report.dateTime), "dd-MM-yyyy"),
        lastChecked: format(new Date(report.lastChecked), "dd-MM-yyyy hh:mm"),
        parentSiteIdInBuild: site.instance === "Build" ? site.parentSiteId : "",
        parentSiteIdInProd:
          site.instance === "Production" ? site.parentSiteId : "",
        parentSiteIdInUAT: site.instance === "UAT" ? site.parentSiteId : "",
        siteName: site.siteName,
      }))
    );

  function mergeReports(obj: SiteReport[], name: string): SiteReport[] {
    const arr: SiteReport[] = [];

    if (name === "SiteNameConflicts") {
      for (let i = 0; i < obj.length - 1; i++) {
        if (obj[i].siteId === obj[i + 1].siteId) {
          arr.push({
            ...obj[i],
            siteNameInBuild: "N/A",
            siteNameInProd: obj[i + 1].siteNameInProd,
            siteNameInUAT: obj[i].siteNameInUAT,
          });
        }
      }
      return arr;
    }
    for (let i = 0; i < obj.length - 1; i++) {
      if (obj[i].siteName === obj[i + 1].siteName) {
        arr.push({
          ...obj[i],
          parentSiteIdInBuild: "N/A",
          parentSiteIdInProd: obj[i + 1].parentSiteIdInProd,
          parentSiteIdInUAT: obj[i].parentSiteIdInUAT,
        });
      }
    }

    return arr;
  }

  const reportsData = [
    {
      title: "Site Conflicts",
      description:
        "These sites are missing in Production, as they are present in Build. If they are attached to a study in Build, you will not be able to deploy that study to Production.",
      columns: siteConflictsReports,
    },
    {
      title: "Site Name Conflicts",
      description:
        "These sites have different names in each instance, but the same Global Site Id. This could result in studies being deployed to incorrect sites.",
      columns: mergeReports(siteNameConflictsReports, "SiteNameConflicts"),
    },
    {
      title: "Site Parent Id Conflicts",
      description:
        "These sites have different Parents in each instance. If they are attached to a study in Build, you will not be able to deploy that study to Production.",
      columns: mergeReports(
        siteParentIdConflictsReports,
        "SiteParentIdConflicts"
      ),
    },
  ];

  return (
    <>
      <h1 className={h1()}>Site Reports</h1>
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
          {reportsData.map((report, index) => (
            <TabsTrigger
              key={index}
              className={triggerStyle}
              value={report.title}
            >
              {report.title}
            </TabsTrigger>
          ))}
        </TabsList>
        {reportsData.map((report, index) => (
          <TabsContent key={index} value={report.title}>
            {report.columns.length <= 0 ? (
              <Alert
                css={{ m: "50px auto" }}
                message="There are currently no reports."
              />
            ) : (
              <div className={vstack({ gap: "6" })}>
                <p className={css({ m: "30px 0px" })}>{report.description}</p>
                <div className={flex({ gap: "6", direction: "column" })}>
                  <DataTable
                    columns={
                      report.title === "Site Name Conflicts"
                        ? siteNameConflictsColumns
                        : report.title === "Site Parent Id Conflicts"
                        ? siteParentIdConflictsColumns
                        : siteConflictsColumns
                    }
                    data={report.columns}
                  />
                </div>
              </div>
            )}
          </TabsContent>
        ))}
      </Tabs>
    </>
  );
}
