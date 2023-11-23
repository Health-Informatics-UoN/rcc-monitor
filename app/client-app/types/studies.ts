import { User } from "@/types/users";

export interface StudyPartial {
  id: number;
  name: string;
  instance: "Production" | "Build" | "UAT";
  users: User[];
  studyCapacityAlert: boolean;
}

export interface Study extends StudyPartial {
  apiKey: string;
}

export interface FormState {
  message: string;
  success: boolean;
}
