import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  triggerStyle,
} from "@/components/ui/tabs";
import { Alert } from "@/components/Alert";
import { getReports } from "@/lib/api/reports";
import { Metadata } from "next";
import { vstack } from "@/styled-system/patterns";
import { css } from "@/styled-system/css";
import ReportsTable from "@/components/reports/ReportsTable";
import { format } from "date-fns";
import { h1 } from "@/styled-system/recipes";

export const metadata: Metadata = {
  title: "RedCap Site Reports",
};

interface SiteNameConflictsReportsRow {
  dateTime: string;
  status: string;
  siteId: string;
  siteName?: string;
  siteNameInBuild: string;
  siteNameInProd: string;
  siteNameInUAT: string;
}

interface SiteParentConflictsReportsRow {
  dateTime: string;
  status: string;
  siteId?: string;
  parentSiteIdInBuild: string;
  parentSiteIdInProd: string;
  parentSiteIdInUAT: string;
  siteName: string;
}

export default async function Reports() {
  const reports = await getReports();

  const siteConflictsReports = reports.filter(
    (v) => v.reportType.name == "ConflictingSite"
  );
  const siteNameConflictsReports = reports.filter(
    (v) => v.reportType.name == "ConflictingSiteName"
  );
  const siteParentConflictsReports = reports.filter(
    (v) => v.reportType.name == "ConflictingSiteParent"
  );

  const siteNameConflictsReportsRow = siteNameConflictsReports.map((row) => {
    const mergedRows: { [key: string]: SiteNameConflictsReportsRow } = {};

    // Loop through the sites and merge rows with the same dateTime, status, and siteId
    row.sites.forEach((site) => {
      const key = `${row.dateTime}_${row.status.name}_${site.siteId}`;
      if (!mergedRows[key]) {
        mergedRows[key] = {
          dateTime: format(new Date(row.dateTime), "yyyy-MM-dd HH:mm:ss"),
          status: row.status.name,
          siteId: site.siteId,
          siteNameInBuild: "N/A",
          siteNameInProd: "N/A",
          siteNameInUAT: "N/A",
        };
      }

      // Update site names based on the instance
      if (site.instance === "Build") {
        mergedRows[key].siteNameInBuild = site.siteName;
      } else if (site.instance === "Production") {
        mergedRows[key].siteNameInProd = site.siteName;
      } else if (site.instance === "UAT") {
        mergedRows[key].siteNameInUAT = site.siteName;
      }
    });

    // Convert the mergedRows object into an array of values
    return Object.values(mergedRows);
  });

  const siteParentConflictsReportsRow = siteParentConflictsReports.map(
    (row) => {
      const mergedRows: { [key: string]: SiteParentConflictsReportsRow } = {};

      row.sites.forEach((site) => {
        const key = `${row.dateTime}_${row.status.name}`;
        if (!mergedRows[key]) {
          mergedRows[key] = {
            dateTime: format(new Date(row.dateTime), "yyyy-MM-dd HH:mm:ss"),
            status: row.status.name,
            parentSiteIdInBuild: "N/A",
            parentSiteIdInProd: "N/A",
            parentSiteIdInUAT: "N/A",
            siteName: site.siteName,
          };
        }

        if (site.instance === "Build") {
          mergedRows[key].parentSiteIdInBuild = site.parentSiteId;
        } else if (site.instance === "Production") {
          mergedRows[key].parentSiteIdInProd = site.parentSiteId;
        } else if (site.instance === "UAT") {
          mergedRows[key].parentSiteIdInUAT = site.parentSiteId;
        }
      });

      return Object.values(mergedRows);
    }
  );

  const reportsObj = {
    siteConflicts: {
      title: "Site Conflicts",
      description:
        "These sites are missing in Production, as they are present in Build. If they are attached to a study in Build, you will not be able to deploy that study to Production.",
      report: {
        columns: ["Time Occured", "Status", "Site Id", "Site Name"],
        rows: siteConflictsReports.map((row) =>
          row.sites.map((site) => ({
            dateTime: format(new Date(row.dateTime), "yyyy-MM-dd HH:mm:ss"),
            status: row.status.name,
            siteId: site.siteId,
            siteName: site.siteName,
          }))
        ),
      },
    },
    siteNameConflicts: {
      title: "Site Name Conflicts",
      description:
        "These sites have different names in each instance, but the same Global Site Id. This could result in studies being deployed to incorrect sites.",
      report: {
        columns: [
          "Time Occured",
          "Status",
          "Site Id",
          "Site Name - Build",
          "Site Name - Production",
          "Site Name - UAT",
        ],
        rows: siteNameConflictsReportsRow,
      },
    },
    siteConflictsParents: {
      title: "Site Parent Id Conflicts",
      description:
        "These sites have different Parents in each instance. If they are attached to a study in Build, you will not be able to deploy that study to Production.",
      report: {
        columns: [
          "Time Occured",
          "Status",
          "Parent Site Id - Build",
          "Parent Site Id - Production",
          "Parent Site Id - UAT",
          "Site Name",
        ],
        rows: siteParentConflictsReportsRow,
      },
    },
  };

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
          {Object.values(reportsObj).map((obj, idx) => (
            <TabsTrigger key={idx} className={triggerStyle} value={obj.title}>
              {obj.title}
            </TabsTrigger>
          ))}
        </TabsList>
        {Object.values(reportsObj).map((obj, idx) => (
          <TabsContent key={idx} value={obj.title}>
            {obj.report.rows.length <= 0 ? (
              <Alert
                css={{ m: "50px auto" }}
                message="There are currently no reports."
              />
            ) : (
              <div className={vstack({ gap: "6" })}>
                <p className={css({ m: "30px 0px" })}>{obj.description}</p>
                <ReportsTable
                  columns={obj.report.columns}
                  rows={obj.report.rows.flat()}
                />
              </div>
            )}
          </TabsContent>
        ))}
      </Tabs>
    </>
  );
}
