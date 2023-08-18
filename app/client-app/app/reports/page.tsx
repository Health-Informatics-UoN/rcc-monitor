import "@/styles/reports.css";
import AlertBar from "@/components/Alert";
import { getReports } from "@/api/reports";
import { Metadata } from 'next';
import { hstack, vstack } from "../../styled-system/patterns";
import { Card, CardContent, CardDescription, CardTitle, CardFooter, CardHeader } from "@/components/ui/card";
import { css } from "../../styled-system/css";

export const metadata: Metadata = {
	title: 'RedCap Site Reports',
};

export default async function Reports() {
  const reports = await getReports();

  const siteConflicts = reports.filter(v => v.reportType.name == "ConflictingSite")
  const siteConflictsNames = reports.filter(v => v.reportType.name == "ConflictingSiteName")
  const siteConflictsParents = reports.filter(v => v.reportType.name == "ConflictingSiteParent")

  return (
    <>
      <h1 className="heading">Site Reports</h1>
      {reports.length <= 0 ? (
        <>
          <AlertBar message="There are currently no reports." />
        </>
      ) : (
        <>
          {siteConflicts.length > 0 &&
            <div className={vstack({ gap: '6' })}>
              <h4 className={css({ textStyle: 'h4'})}>Site Conflicts</h4>
              <p>{`These sites are missing in Production, as they are present in Build. 
              If they are attached to a study in Build, you will not be able to deploy that study to Production.`}
              </p>
              {siteConflicts.map((report) => (
                <ReportCard key={report.id} report={report} />
                ))}
            </div>
          }

          {siteConflictsNames.length > 0 &&
            <div className={vstack({ gap: '6' })}>
              <h4 className={css({ textStyle: 'h4'})}>Site Name Conflicts</h4>
              <p>{`These sites have different names in each instance, but the same Global Site Id. This could result in studies being deployed to incorrect sites.`}</p>
              {siteConflictsNames.map((report) => (
                <ReportCard key={report.id} report={report} />
                ))}
            </div>
          }

          {siteConflictsParents.length > 0 &&
            <div className={vstack({ gap: '6' })}>
              <p>{
              `These sites have different "Parents" in each instance. 
              If they are attached to a study in Build, you will not be able to deploy that study to Production.`
              }</p>
              <h4 className={css({ textStyle: 'h4'})}>Site Parent Conflicts</h4>
              {siteConflictsParents.map((report) => (
                <ReportCard key={report.id} report={report} />
                ))}
            </div>
          }
        </>
      )}
    </>
  );
}

const ReportCard = ({report} : {report : ReportModel}) => {
  return(
    <Card>
      <CardContent>
        <div className={hstack({ gap: '2'})}>
          <div>
            <p>
              {report.status.name}
            </p>
            {new Date(report.dateTime).toLocaleDateString() +
              " " +
              new Date(report.dateTime).toLocaleTimeString()}
          </div>
          {report.sites.map((site) => {
            return(
              <div key={site.instance}>
                <strong>Instance: {site.instance}</strong>
                <p>Site Id: {site.siteId}</p>
                <p>Site Id: {site.siteName}</p>
                <p>Parent Site Id: {site.parentSiteId}</p>
              </div>
            )
          })}
          <p>{report.description}</p>
        </div>
      </CardContent>
    </Card>
  )
}
