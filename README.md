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

When setting up a new environment, you need to import the Keycloak realm, found in `keycloak/keycloak-realm.json`. This realm contains a client application for the Nextjs client currently.  

## 📁 Repository contents

Areas within this repo include:

- Application Source Code
  - .NET7 backend API
  - Azure Functions App
  - Shared Data class library

## App Configuration

Notes on configuration values that can be provided, and their defaults.

The app can be configured in any standard way an ASP.NET Core application can. Typically from the Azure Portal (Environment variables) or an `appsettings.json`.

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

  UserAccounts:
    SendEmail: # true or false. if true, sends an email to the user
    GenerateLink: # true or false. if true, generates link to the client
  # the above two options are appicable with account activation (user invite) including resending and changing password.

  Registration":
    UseAllowlist: # true or false. If true, checks if email is in the RegistrationAllowlist table.
    UseRules: # true or false. If true, checks if email satisfies the registration rules.
    # the above options are curently used in CanRegister method, which determines whether a given email can register or not.

    AllowList: [] # String array containing email/domain that are allowed to register. Example ["@example.com", "allow@example1.com"]
    BlockList: [] # String array containing email/domain that are blocked from registration. Example ["block@example.com", "@example1.com"]
```

The app frontend can be configured similarly using Azure Portal (Environment variables), or locally using a `.env.local` file.

```bash
API_URL=https://localhost:7007
NODE_TLS_REJECT_UNAUTHORIZED='0'
KEYCLOAK_ID=frontend
KEYCLOAK_SECRET=l0ruWXoSTTPs7lp7Vpuv2N0ivTEYphxq
KEYCLOACK_ISSUER=http://localhost:9080/realms/nuh
NEXTAUTH_URL=http://localhost:3000
NEXTAUTH_SECRET=changeme
```
