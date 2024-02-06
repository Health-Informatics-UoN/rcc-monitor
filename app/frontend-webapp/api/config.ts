"use server";
import { revalidatePath } from "next/cache";

import request from "@/lib/api/request";
import {
  ConfigModel,
  FeatureFlagModel,
  UpdateConfigModel,
} from "@/types/config";

const fetchKeys = {
  list: "config",
  getFlags: "config/flags",
  update: (configKey: string) => `config/${configKey}`,
};

export async function getFeatureFlags(): Promise<FeatureFlagModel> {
  try {
    return await request<FeatureFlagModel>(fetchKeys.getFlags);
  } catch (error) {
    console.warn("Failed to fetch data.");
    return {
      siteMonitoringEnabled: false,
      syntheticDataEnabled: false,
      studyManagementEnabled: false,
    };
  }
}

export async function getSiteConfig(): Promise<ConfigModel[]> {
  try {
    return await request<ConfigModel[]>(fetchKeys.list);
  } catch (error) {
    console.warn("Failed to fetch data.");
    return [];
  }
}

export async function updateSiteConfig(model: UpdateConfigModel) {
  await request<UpdateConfigModel>(fetchKeys.update(model.key), {
    method: "PUT",
    headers: {
      "Content-type": "application/json",
    },
    body: JSON.stringify(model),
  });
  revalidatePath("/settings");
}
