import { User } from "@/types/users";

export interface StudyPartial {
  id: number;
  name: string;
  instance: "Production" | "Build" | "UAT";
  users: User[];
  studyGroup: StudyGroup[];
  studyCapacityAlert: boolean;
  studyCapacityAlertsActivated: boolean;
  studyCapacityThreshold: number;
  studyCapacityJobFrequency: string;
  studyCapacityLastChecked: string;
}

export interface StudyGroup {
  id: number;
  name: string;
  plannedSize: string;
}

export interface Study extends StudyPartial {
  apiKey: string;
}

export interface FormState {
  message: string;
  success: boolean;
}
