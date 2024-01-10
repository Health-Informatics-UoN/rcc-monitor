// This is a standard web app App Service template
// with some useful conditional features

// It does the following:
// - Create App Insights resource against a Log Analytics workspace, for use with the app
// - Create an App Service against an App Service Plan
//   - defaults are for .NET LTS but overridable
// - if a vnet subnet id is provided, sets up vnet integration on that subnet
//   - note than a minimum asp sku of S1 is required to support this
// - if custom hostnames are provided, configures them (without SSL - see below)
//   - DNS verification will need to already have occurred to approve the subscription
//   - this takes the form of an `asuid` subdomain TXT record and is well documented

// It does NOT do the following
// it's expected they would be done after the fact if desired
// - App Settings - could be dependent on key vault access
// - Connection Strings - could be dependent on key vault access
// - SSL Certs - needs to be done in a separate module to custom hostnames for some reason

param appHostnames array = []
param appName string
param aspName string
param logAnalyticsWorkspaceName string
param tags object = {}

param appFramework string = 'DOTNETCORE|8.0' // az webapp list-runtimes --os-type linux

param location string = resourceGroup().location

resource asp 'Microsoft.Web/serverfarms@2020-10-01' existing = {
  name: aspName
}

resource laWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource appinsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${appName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'LogAnalytics' // "new" AppInsights
    RetentionInDays: 90
    WorkspaceResourceId: laWorkspace.id
  }
  tags: union({
      Source: 'Bicep'
    }, tags)
}

var frameworkParts = split(appFramework, '|')
var frameworkRuntime = frameworkParts[0]
var frameworkVersion = frameworkParts[1]

resource app 'Microsoft.Web/sites@2022-09-01' = {
  name: appName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: asp.id
    enabled: true
    siteConfig: {
      netFrameworkVersion: frameworkRuntime == 'DOTNETCORE' ? 'v${frameworkVersion}' : ''
      linuxFxVersion: appFramework
      requestTracingEnabled: true
      httpLoggingEnabled: true
      use32BitWorkerProcess: true
      alwaysOn: true
      managedPipelineMode: 'Integrated'
      virtualApplications: [
        {
          virtualPath: '/'
          physicalPath: 'site\\wwwroot'
          preloadEnabled: true
        }
      ]
      http20Enabled: true
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
  tags: union({
      Source: 'Bicep'
    }, tags)
}

@batchSize(1) // Hostnames need to be done serially
resource hostnameBinding 'Microsoft.Web/sites/hostNameBindings@2021-02-01' = [for hostname in appHostnames: {
  name: hostname
  parent: app
  properties: {
    siteName: appName
    sslState: 'Disabled'
    hostNameType: 'Verified'
    customHostNameDnsRecordType: 'CName'
  }
}]

// Outputs

type AppServiceIdentityOutputs = {
  tenantId: string
  principalId: string
}

type AppServiceAppInsightsOutputs = {
  name: string
  instrumentationKey: string
}

output name string = appName
output aspId string = asp.id
output identity AppServiceIdentityOutputs = {
  tenantId: app.identity.tenantId
  principalId: app.identity.principalId
}
output appInsights AppServiceAppInsightsOutputs = {
  name: appinsights.name
  instrumentationKey: appinsights.properties.InstrumentationKey
}
