# Introduction

This repository is for the NUH Collaboration projects, currently focussed on monitoring the health of instances of RedCap Cloud.

This currently consists of an web application backend, and Azure Functions app, that interact with a PostgreSQL database.

## Getting Started

## Prerequisites

1. **.NET SDK** `7.x`
   - The backend API is .NET7
1. Docker

## Database setup

The application stack interacts with a PostgreSQL Server database, and uses code-first migrations for managing the database schema.

The repository contains a `docker-compose` for the database, so just run `docker-compose up -d` to start it running.

When setting up a new environment, or running a newer version of the codebase if there have been schema changes, you need to run migrations against your database server.

The easiest way is using the dotnet cli:

1. If you haven't already, install the local Entity Framework tooling

- Anywhere in the repo: `dotnet tool restore`

1. Navigate to the same directory as `Monitor.csproj`
1. Run migrations:

- `dotnet ef database update`
- The above runs against the default local server, using the connection string in `appsettings.Development.json`
- You can specify a connection string with the `--connection "<connection string>"` option

## Authentication setup

We use Keycloak for authentication, and the service runs as part of the `docker-compose`.

When setting up a new environment, you need to import the Keycloak realm, found in `keycloak/nuh-dev.json`. This realm contains the client applications for backend and frontend, and custom roles.

In production, the Kecloak secret for the client and backend will need to be regenerated and replaced in the environment variable and appsettings respectively, as we do not want to use default or development values.

## Storage Setup

We use Azure Blob storage for storing files, so use the Azurite for local development, this service runs as part of the `docker-compose`.

You will need to add a container for the files, using the Azure CLI:

`az storage container create --name synthetic-data  --connection-string "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"`

## 📁 Repository contents

Areas within this repo include:

- Application Source Code
  - .NET7 backend API
  - Azure Functions App
  - Shared Data class library

## App Configuration

Notes on configuration values that can be provided, and their defaults.

The backend app can be configured in any standard way an ASP.NET Core application can. Typically from the Azure Portal (Environment variables) or an `appsettings.json`.

```yaml
OutboundEmail:
  ServiceName: RedCap Monitor
  FromName: No Reply
  FromAddress: noreply@example.com
  ReplyToAddress: ""
  Provider: local

  # If Provider == "local"
  LocalPath: /temp

  # If Provider == "sendgrid"
  SendGridApiKey: ""

  # CORS URL
  FrontendAppUrl: "http://localhost:3000"

  Keycloak:
    realm: "nuh-uat",
    auth-server-url: "http://localhost:9080/",
    ssl-required: "none",
    resource: "backend",
    public-client: true,
    verify-token-audience: false,
    confidential-port: 0,
    credentials:
      secret: ""
    RolesSource: "Realm"
```

The frontend app can be configured in any standard way an Node application can. Typically from the Azure Portal (Environment variables) or an `.env.local`.

```bash
BACKEND_URL=https://localhost:7007
NODE_TLS_REJECT_UNAUTHORIZED='0' # For development
KEYCLOAK_ID=frontend
KEYCLOAK_SECRET=changeme
KEYCLOACK_ISSUER=http://localhost:9080/realms/nuh-dev
NEXTAUTH_URL=http://localhost:3000
NEXTAUTH_SECRET=changeme
NEXT_PUBLIC_REDCAP_BUILD_URL=https://eubuild.redcapcloud.com
NEXT_PUBLIC_REDCAP_PROD_URL=https://nuh.eulogin.redcapcloud.com
NEXT_PUBLIC_REDCAP_UAT_URL=https://uat.redcapcloud.com
```
