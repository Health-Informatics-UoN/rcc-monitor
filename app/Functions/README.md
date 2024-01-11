# NUH Function App

This project contains the Azure functions for the NUH application.

Currently, these are:

- `SiteDiffReportJob`
- `StudyCapacityJob`
- `StudyProductionDataJob`
- `SendSummaryEmail`

## `SiteDiffReportJob`

This function runs once a day, to retrieve the RedCap Cloud tenant level Sites from two environments, and report warnings. This is necessary, as the sites need to be identical in the two environments to enable studies to be imported between them.

These warnings are:

- If a site exists in one environment (UAT), but not the other (build).
- If a site is named differently in the two environments, but has the same `Global Site ID`.
- If a site has a different parent in the two environments.

## `StudyCapacityJob`

This function runs every hour, to check whether the RedCap Production studies have reach their capacity threshold, as set by the Study Groups. This is necessary, as a Study can reach its capacity without noticing.

## `StudyProductionDataJob`

This function runs every day at 9am, to check if RedCap Build Studies might have real world data being entered. This is necessary, as it is possible for users to mistake the build instance for a production, and enter real world data.

## `SendSummaryEmail`

This function runs once a day, to tell an endpoint on the Webapp to send the Site Report summary emails.

## Development

### Prerequisites

- Docker
- .NET SDK `8.x`

### Running the functions

Run the Azurite from the repo root docker-compose: `docker-compose up -d`

Your IDE should recognise the functions, and be able to run them.

If you're using the command line, run `func start`.

## Configuration

The following values are needed to configure the application, in Azure Portal these are the environment variables.
In local development a `local.settings.json` file is required, with the following values.

```json
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsEnv": "Development",
    "UseRedCapData": false, # Configure whether to fetch live data from RedCap, or use the local service.
    "UseEmailReports": true,
    "RedCap:ProductionKey": "", # Tenant level API token
    "RedCap:UATKey": "", # Tenant level API token
    "RedCap:ProductionUrl": "https://nuh.eulogin.redcapcloud.com",
    "RedCap:UATUrl": "http://eubuild.redcapcloud.com",
    },
     "ConnectionStrings": {
        "postgres": "Host=localhost;Username=postgres;Port=5432;Password=example;Database=monitor"
    }
}
```
