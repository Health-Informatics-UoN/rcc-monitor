# RCC-Monitor

RCC-Monitor is a web application for monitoring the health of instances, and related studies on RedCap Cloud.

## Getting Started

User and Developer Guidance can be found in the [documentation](https://health-informatics-uon.github.io/rcc-monitor/).

## Repository contents

| Path                  | Description                | Notes                                                                                      |
| --------------------- | -------------------------- | ------------------------------------------------------------------------------------------ |
| `app/Monitor`         | .NET8 Backend              |                                                                                            |
| `app/frontend-webapp` | Next.js Frontend           |                                                                                            |
| `app/Functions`       | .NET8 Azure Functions      | Worker functions.                                                                          |
| `lib/Data`            | .NET8 Data Class Library   | Shared data access.                                                                        |
| `lib/Monitor.Shared`  | .NET8 Shared Class Library | Shared code library.                                                                       |
| `.azure`              | Azure Bicep Files          | Files for deploying application infrastructure.                                            |
| `.github`             | GitHub Actions             | workflows for building and deploying the applications                                      |
| `docs`                | Nextra Docs Site           | The source for the [documentation](https://health-informatics-uon.github.io/rcc-monitor/). |
