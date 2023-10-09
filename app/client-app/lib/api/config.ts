import request from "./request";

interface ConfigModel {
  siteMonitoringEnabled: boolean;
}

const fetchKeys = {
  list: "config",
};

export async function getServerConfig(): Promise<ConfigModel> {
  try {
    return await request<ConfigModel>(fetchKeys.list);
  } catch (error) {
    console.warn("Failed to fetch data.");
    return { siteMonitoringEnabled: false };
  }
}
