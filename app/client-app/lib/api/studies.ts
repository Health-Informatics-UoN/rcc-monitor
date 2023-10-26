import { StudyPartial } from "@/types/studies";
import request from "./request";

const fetchKeys = {
  list: "studies",
};

export async function getStudies(): Promise<StudyPartial[]> {
  try {
    return await request<StudyPartial[]>(fetchKeys.list);
  } catch (error) {
    console.warn("Failed to fetch data.");
    return [];
  }
}
