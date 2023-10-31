export interface StudyPartial {
  id: number;
  name: string;
}

export interface Study extends StudyPartial {
  apiKey: string;
}

export interface FormState {
  message: string;
  success: boolean;
}
