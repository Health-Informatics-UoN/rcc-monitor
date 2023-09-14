param appName string

// Key Value pairs in here
param appSettings object = {}

type ConnectionStringDictionary = {
  *: {
    value: string
    type: 'SQLServer' | 'Custom'
  }
}
param connectionStrings ConnectionStringDictionary = {}

// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/sites/config-appsettings
resource settings 'Microsoft.Web/sites/config@2020-09-01' = if (!empty(appSettings)) {
  name: '${appName}/appsettings'
  properties: appSettings
}

// https://learn.microsoft.com/en-us/azure/templates/microsoft.web/sites/config-connectionstrings?pivots=deployment-language-bicep
resource connectionstrings 'Microsoft.Web/sites/config@2020-09-01' = if (!empty(connectionStrings)) {
  name: '${appName}/connectionstrings'
  properties: connectionStrings
}
