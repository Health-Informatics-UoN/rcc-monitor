param keyVaultName string
param tenantId string
param objectId string

resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' existing = {
  name: keyVaultName
}


resource kvAccess 'Microsoft.KeyVault/vaults/accessPolicies@2023-02-01' = {
  name: 'add'
  parent: kv
  properties: {
    accessPolicies: [
      {
        tenantId: tenantId
        objectId: objectId
        permissions: {
          secrets: [
            'get'
          ]
        }
      }
    ]
  }
}
