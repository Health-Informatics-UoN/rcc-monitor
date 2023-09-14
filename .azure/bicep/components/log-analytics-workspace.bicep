param logAnalyticsWorkspaceName string
param tags object = {}

param location string = resourceGroup().location

resource laWorkspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
  tags: union({
    Source: 'Bicep'
  }, tags)
}

output name string = laWorkspace.name
output id string = laWorkspace.id
