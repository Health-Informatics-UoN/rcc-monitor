// This deploys an entire environment stack
// It reuses some shared resources within a resource group (e.g. prod / non-prod)
// and then deploys and configures environment specific resources
// based on parameters passed through
// for a given service and environment combination (e.g. monitor dev)

// USE PARAMETER FILES to deploy an actual environment

func referenceSecret(vaultName string, secretName string) string => '@Microsoft.KeyVault(VaultName=${vaultName};SecretName=${secretName})'

type ServiceNames = 'monitor'
param serviceName ServiceNames

type Environments = 'dev' | 'qa' | 'uat' | 'prod'
param env Environments

param appName string = '${env}-${serviceName}'
param backendHostnames array = []
param backendAppSettings object = {}

param frontendHostnames array = []
param keyVaultName string = '${serviceName}-${env}'

param location string = resourceGroup().location

param sharedEnv string = 'shared'
var sharedPrefix = resourceGroup().name

param appServicePlanSku string = 'S1'

// Shared Resources
// ensure they are present,
// even though they might be there
// due to sharing with other environments

// log analytics workspace
module la 'components/log-analytics-workspace.bicep' = {
  name: 'la-ws-${uniqueString(sharedPrefix)}'
  params: {
    location: location
    logAnalyticsWorkspaceName: '${sharedPrefix}-la-ws'
    tags: {
      Environment: sharedEnv
    }
  }
}

// App Service Plan
module asp 'components/app-service-plan.bicep' = {
  name: 'asp'
  params: {
    location: location
    aspName: '${sharedPrefix}-asp'
    sku: appServicePlanSku
    tags: {
      Environment: sharedEnv
    }
  }
}

// Per Environment Resources

// Environment Key Vault pre-existing and populated
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultName
}


// Create a storage account for worker operations
// And add its connection string to keyvault :)
// Blob and Queue reads/writes/triggers use this account
module workerStorage 'components/storage-account.bicep' = {
  name: 'storage-${uniqueString(appName)}'
  params: {
    location: location
    baseAccountName: 'worker'
    keyVaultName: kv.name
    uniqueStringSource: appName
    tags: {
      Service: serviceName
      Environment: env
    }
  }
}

// Create the Backend App and related bits
// App Insights
// App Service
// Hostnames
module backend 'components/web-app-service.bicep' = {
  name: 'backend-${uniqueString(appName)}'
  params: {
    location: location
    appName: appName
    aspName: asp.outputs.name
    logAnalyticsWorkspaceName: la.outputs.name
    // appHostnames: backendHostnames

    tags: {
      Service: serviceName
      Environment: env
    }
  }
}

// Create the Frontend App and related bits
// App Insights
// App Service
// Hostnames
module frontend 'components/web-app-service.bicep' = {
  name: 'frontend-${uniqueString(appName)}'
  params: {
    location: location
    appName: '${appName}-frontend'
    aspName: asp.outputs.name
    logAnalyticsWorkspaceName: la.outputs.name
    // appHostnames: frontendHostnames
    appFramework: 'NODE|18-LTS'

    tags: {
      Service: serviceName
      Environment: env
    }
  }
}

// Grant the backend Key Vault access
module backendKvAccess 'config/keyvault-access.bicep' = {
  name: 'kvAccess-${uniqueString(appName)}'
  params: {
    keyVaultName: kv.name
    tenantId: backend.outputs.identity.tenantId
    objectId: backend.outputs.identity.principalId
  }
}

// Config (App Settings, Connection strings) here now that Key Vault links will resolve
// Overrides for environments come through as params

// Shared configs are defined inline here
var appInsightsSettings = {
  // App Insights
  APPINSIGHTS_INSTRUMENTATIONKEY: backend.outputs.appInsights.instrumentationKey
  ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
  XDT_MicrosoftApplicationInsights_Mode: 'recommended'
  DiagnosticServices_EXTENSION_VERSION: '~3'
  APPINSIGHTS_PROFILERFEATURE_VERSION: '1.0.0'
  APPINSIGHTS_SNAPSHOTFEATURE_VERSION: '1.0.0'
  InstrumentationEngine_EXTENSION_VERSION: '~1'
  SnapshotDebugger_EXTENSION_VERSION: '~1'
  XDT_MicrosoftApplicationInsights_BaseExtensions: '~1'
}

var friendlyEnvironmentNames = {
  dev: 'Dev'
  qa: 'QA'
  uat: 'UAT'
  prod: 'Production'
}
var baseBackendSettings = {
  DOTNET_Environment: friendlyEnvironmentNames[env]

  OutboundEmail__Provider: 'sendgrid'
  OutboundEmail__SendGridApiKey: referenceSecret(kv.name, 'sendgrid-api-key')

  JWT__Secret: referenceSecret(kv.name, 'api-jwt-secret')
}

module backendSiteConfig 'config/app-service-config.bicep' = {
  name: 'siteConfig-${uniqueString(appName)}'
  params: {
    appName: backend.outputs.name
    appSettings: union(
      appInsightsSettings,
      baseBackendSettings,
      backendAppSettings)
    connectionStrings: {
      Default: {
        type: 'Custom'
        value: referenceSecret(kv.name, 'db-connection-string')
      }
      AzureStorage: {
        type: 'Custom'
        value: referenceSecret(kv.name, workerStorage.outputs.connectionStringKvRef)
      }
    }
  }
}

var baseFrontendSettings = {
  API_URL: 'https://${backend.outputs.name}.azurewebsites.net'
}

module frontendSiteConfig 'config/app-service-config.bicep' = {
  name: 'siteConfig-${uniqueString(frontend.name)}'
  params: {
    appName: frontend.outputs.name
    appSettings: union(
      appInsightsSettings,
      baseFrontendSettings)
  }
}

// Function Apps
module functionApp 'components/functions-app.bicep' = {
  name: 'function-${uniqueString(appName)}'
  params: {
    location: location
    aspName: asp.outputs.name
    appName: '${appName}-worker'
    logAnalyticsWorkspaceName: la.outputs.name
    tags: {
      FriendlyIdentifier: serviceName
      Environment: env
    }
  }
}

// grant the worker keyvault access for any settings it needs
module workerKvAccess 'config/keyvault-access.bicep' = {
  name: 'kvAccess-${uniqueString(functionApp.name)}'
  params: {
    keyVaultName: keyVaultName
    tenantId: functionApp.outputs.tenantId
    objectId: functionApp.outputs.principalId
  }
}

// Add settings
//(now that keyvault is accessible, if any are KeyVault linked)
module workerAppSettings 'config/function-app-config.bicep' = {
  name: 'functionConfig-${uniqueString(appName)}'
  params: {
    appName: functionApp.outputs.name
    keyVaultName: keyVaultName
    appInsightsName: functionApp.outputs.appInsightsName
    apiStorageConnectionStringKvRef: workerStorage.outputs.connectionStringKvRef
  }
}

// Add SSL certificates
// this needs to be done as a separate stage to creating the app with a bound hostname
@batchSize(1) // also needs to be done serially to avoid concurrent updates to the app service
module backendApiCert 'components/managed-cert.bicep' = [for hostname in backendHostnames: {
  name: 'api-cert-${uniqueString(hostname)}'
  params: {
    location: location
    hostname: hostname
    appName: backend.outputs.name
    aspId: backend.outputs.aspId
  }
}]

// Add SSL certificates
// this needs to be done as a separate stage to creating the app with a bound hostname
@batchSize(1) // also needs to be done serially to avoid concurrent updates to the app service
module frontendApiCert 'components/managed-cert.bicep' = [for hostname in frontendHostnames: {
  name: 'api-cert-${uniqueString(hostname)}'
  params: {
    location: location
    hostname: hostname
    appName: backend.outputs.name
    aspId: backend.outputs.aspId
  }
}]
