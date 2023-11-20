using '../main.bicep'

param serviceName = 'monitor'
param env = 'uat'

param backendHostnames = []

param backendAppSettings = union(
  loadYamlContent('base.appsettings.yaml'),
  {
    FeatureManagement: {
      SiteMonitoring: true
      SyntheticData: true
      StudyManagement: true
    }
  }
)

