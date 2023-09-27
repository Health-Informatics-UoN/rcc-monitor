import Alert from "@/components/Alert";
import { getReports } from "@/lib/api/reports";
import { Metadata } from "next";
import { vstack } from "@/styled-system/patterns";
import { css } from "@/styled-system/css";
import ReportsTable from "@/components/reports/ReportsTable";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  triggerStyle,
} from "@/components/ui/tabs";

export const metadata: Metadata = {
  title: "RedCap Site Reports",
};

export default async function Reports() {
  const reports = await getReports();

  const reportsObj = {
    siteConflicts: {
      title: "Site Conflicts",
      report: reports.filter((v) => v.reportType.name == "ConflictingSite"),
      description:
        "These sites are missing in Production, as they are present in Build. If they are attached to a study in Build, you will not be able to deploy that study to Production.",
    },
    siteNameConflicts: {
      title: "Site Name Conflicts",
      report: reports.filter((v) => v.reportType.name == "ConflictingSiteName"),
      description:
        "These sites have different names in each instance, but the same Global Site Id. This could result in studies being deployed to incorrect sites.",
    },
    siteConflictsParents: {
      title: "Site Parent Id Conflicts",
      report: reports.filter(
        (v) => v.reportType.name == "ConflictingSiteParent"
      ),
      description:
        "These sites have different Parents in each instance. If they are attached to a study in Build, you will not be able to deploy that study to Production.",
    },
  };

  return (
    <>
      <h1
        className={css({
          mt: "10px",
          fontSize: "35px",
          fontWeight: "bold",
          textAlign: "center",
        })}
      >
        Site Reports
      </h1>
      <Tabs defaultValue="tab1" w="95%" position="relative" m="30px auto">
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
            {obj.report.length <= 0 ? (
              <Alert
                css={{ m: "50px auto" }}
                message="There are currently no reports."
              />
            ) : (
              <div className={vstack({ gap: "6" })}>
                <p className={css({ m: "30px 0px" })}>{obj.description}</p>
                <ReportsTable reports={obj.report} />
              </div>
            )}
          </TabsContent>
        ))}
      </Tabs>
    </>
  );
}
