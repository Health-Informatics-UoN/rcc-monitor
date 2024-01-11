import { User } from "@/types/users";

export interface StudyPartial {
  id: number;
  name: string;
  instance: "Production" | "Build" | "UAT";
  users: User[];
  studyGroup: StudyGroup[];
  studyCapacityAlert: boolean;
  productionSubjectsEnteredAlert: boolean;
  studyCapacityAlertsActivated: boolean;
  studyCapacityThreshold: number;
  studyCapacityJobFrequency: string;
  studyCapacityLastChecked: string;
  subjectsEnrolled: number;
  subjectsEnrolledThreshold: number;
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
