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

param appServicePlanSku string = 'B3'

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

// Create the Keycloak App and related bits
// App Insights
// App Service
// Hostnames
module keycloak 'components/web-app-service.bicep' = {
  name: 'identity-${uniqueString(appName)}'
  params: {
    location: location
    appName: '${appName}-identity'
    aspName: asp.outputs.name
    logAnalyticsWorkspaceName: la.outputs.name
    appFramework: 'DOCKER|bitnami/keycloak:22-debian-11'

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
    appName: '${appName}-backend'
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

// Grant keycloak Key Vault access
module keycloakKvAccess 'config/keyvault-access.bicep' = {
  name: 'kvAccess-${uniqueString(keycloak.name)}'
  params: {
    keyVaultName: kv.name
    tenantId: keycloak.outputs.identity.tenantId
    objectId: keycloak.outputs.identity.principalId
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

// Grant the frontend Key Vault access
module frontendKvAccess 'config/keyvault-access.bicep' = {
  name: 'kvAccess-${uniqueString(frontend.name)}'
  params: {
    keyVaultName: kv.name
    tenantId: frontend.outputs.identity.tenantId
    objectId: frontend.outputs.identity.principalId
  }
}

// Config (App Settings, Connection strings) here now that Key Vault links will resolve
// Overrides for environments come through as params

// Shared configs are defined inline here
var appInsightsSettings = {
  // App Insights
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

var baseKeycloakSettings = {
  APPINSIGHTS_INSTRUMENTATIONKEY: keycloak.outputs.appInsights.instrumentationKey
  KEYCLOAK_ADMIN_PASSWORD: referenceSecret(kv.name, 'keycloak-admin-password')
  KEYCLOAK_ADMIN_USER: 'identity_admin'
  KEYCLOAK_DATABASE_HOST: referenceSecret(kv.name, 'keycloak-database-host')
  KEYCLOAK_DATABASE_NAME: 'identity'
  KEYCLOAK_DATABASE_PASSWORD: referenceSecret(kv.name, 'keycloak-database-password')
  KEYCLOAK_DATABASE_USER: referenceSecret(kv.name, 'keycloak-database-user')
  KEYCLOAK_FRONTEND_URL: 'https://${keycloak.outputs.name}.azurewebsites.net'
  KC_PROXY: 'edge'
  WEBSITES_PORT: '8080'
}

module keycloakSiteConfig 'config/app-service-config.bicep' = {
  name: 'siteConfig-${uniqueString(keycloak.name)}'
  params: {
    appName: keycloak.outputs.name
    appSettings: union(
      appInsightsSettings,
      baseKeycloakSettings)
  }
}

var baseBackendSettings = {
  DOTNET_Environment: friendlyEnvironmentNames[env]

  OutboundEmail__Provider: 'sendgrid'
  OutboundEmail__SendGridApiKey: referenceSecret(kv.name, 'sendgrid-api-key')

  APPINSIGHTS_INSTRUMENTATIONKEY: backend.outputs.appInsights.instrumentationKey
  JWT__Secret: referenceSecret(kv.name, 'api-jwt-secret')
  Keycloak__realm: 'nuh-${env}'
  Keycloak__authServerUrl: 'https://${keycloak.outputs.name}.azurewebsites.net'
  Keycloak__sslRequired: 'none'
  Keycloak__resource: 'backend'
  Keycloak__publicClient: 'true'
  Keycloak__verifyTokenAudience: 'false'
  Keycloak__credentials__secret: referenceSecret(kv.name, 'backend-keycloak-secret')
  RolesSource: 'Realm'
  FrontendAppUrl: 'https://${frontend.outputs.name}.azurewebsites.net'
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
  APPINSIGHTS_INSTRUMENTATIONKEY: frontend.outputs.appInsights.instrumentationKey
  BACKEND_URL: 'https://${backend.outputs.name}.azurewebsites.net'
  KEYCLOAK_ID: 'frontend'
  KEYCLOAK_SECRET: referenceSecret(kv.name, 'frontend-keycloak-secret')
  KEYCLOAK_ISSUER: 'https://${keycloak.outputs.name}.azurewebsites.net/realms/nuh-${env}'
  NEXTAUTH_URL: 'https://${frontend.outputs.name}.azurewebsites.net'
  NEXTAUTH_SECRET: referenceSecret(kv.name, 'nextauth-secret')
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

// Worker Apps
module workerApp 'components/functions-app.bicep' = {
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
  name: 'kvAccess-${uniqueString(workerApp.name)}'
  params: {
    keyVaultName: keyVaultName
    tenantId: workerApp.outputs.identity.tenantId
    objectId: workerApp.outputs.identity.principalId
  }
}

var baseWorkerSettings = {
  APPINSIGHTS_INSTRUMENTATIONKEY: workerApp.outputs.appInsights.instrumentationKey
  UseRedCapData: 'true'
  RedCap__ProductionUrl: 'https://nuh.eulogin.redcapcloud.com'
  RedCap__ProductionKey: referenceSecret(kv.name, 'redcap-production-key')
  RedCap__UATUrl: 'https://eubuild.redcapcloud.com'
  RedCap__UATKey: referenceSecret(kv.name, 'redcap-uat-key')
  RedCap__ApiUrl: 'https://${backend.outputs.name}.azurewebsites.net/api/'
  Identity__Issuer: 'https://${keycloak.outputs.name}.azurewebsites.net/realms/nuh-${env}/protocol/openid-connect/token'
  Identity__ClientId: 'functions'
  Identity__Secret: referenceSecret(kv.name, 'worker-identity-secret')
}

// Add settings
//(now that keyvault is accessible, if any are KeyVault linked)
module workerAppConfig 'config/function-app-config.bicep' = {
  name: 'functionConfig-${uniqueString(appName)}'
  params: {
    appName: workerApp.outputs.name
    keyVaultName: keyVaultName
    appInsightsName: workerApp.outputs.appInsights.name
    apiStorageConnectionStringKvRef: workerStorage.outputs.connectionStringKvRef
    appSettings: baseWorkerSettings
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
