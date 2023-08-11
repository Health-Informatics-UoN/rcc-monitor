import "@/styles/reports.css";
import AlertBar from "@/components/Alert";
import { getReports } from "@/api/getReports";
import { Center } from "@/styled-system/jsx";

export default async function Reports() {
  const reports: ReportModel[] = await getReports();

  return (
    <>
      <h1 className="heading">Reports</h1>
      {reports.length <= 0 ? (
        <Center>
          <AlertBar message="There are currently no reports" />
        </Center>
      ) : (
        <Center>
          <table className="reports-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Date Time</th>
                <th>Site Name</th>
                <th>Site ID</th>
                <th>Description</th>
                <th>ReportType</th>
                <th>Instance</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {reports.map((report) => (
                <tr key={report.id}>
                  <td>{report.id}</td>
                  <td>
                    {new Date(report.dateTime).toLocaleDateString() +
                      " " +
                      new Date(report.dateTime).toLocaleTimeString()}
                  </td>
                  <td>{report.description}</td>
                  <td>{report.siteName}</td>
                  <td>{report.description}</td>
                  <td>{report.reportType.name}</td>
                  <td>{report.instance.name}</td>
                  <td>{report.status.name}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </Center>
      )}
    </>
  );
}
