export interface FeatureFlagModel {
  siteMonitoringEnabled: boolean;
  syntheticDataEnabled: boolean;
  studyManagementEnabled: boolean;
}

export interface UpdateConfigModel {
  key: string;
  value: string;
}

export interface ConfigModel extends UpdateConfigModel {
  name: string;
  description: string;
}
