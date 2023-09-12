using '../main.bicep'

param serviceName = 'monitor'
param env = 'prod'

param backendHostnames = []

param appServicePlanSku = 'P1v3'

param backendAppSettings = union(
  loadYamlContent('base.appsettings.yaml'),
  {
  }
)
