param appName string
param hostname string
param aspId string

param location string = resourceGroup().location

resource app 'Microsoft.Web/sites@2020-06-01' existing = {
  name: appName
}

resource cert 'Microsoft.Web/certificates@2022-09-01' = {
  name: '${appName}_${hostname}'
  location: location
  properties: {
    canonicalName: hostname
    serverFarmId: aspId
  }
}

// update the existing app's binding with the cert details
resource hostnameBinding 'Microsoft.Web/sites/hostNameBindings@2021-02-01' = {
  name: hostname
  parent: app
  properties: {
    siteName: appName
    sslState: 'SniEnabled'
    hostNameType: 'Verified'
    customHostNameDnsRecordType: 'CName'
    thumbprint: cert.properties.thumbprint
  }
}
