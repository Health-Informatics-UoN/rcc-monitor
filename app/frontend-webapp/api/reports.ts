import { ReportModel } from "@/types";

import request from "./request";

const fetchKeys = {
  list: "reports",
};

export async function getReports(): Promise<ReportModel[]> {
  try {
    return await request<ReportModel[]>(fetchKeys.list);
  } catch (error) {
    console.warn("Failed to fetch data.");
    return [];
  }
}
