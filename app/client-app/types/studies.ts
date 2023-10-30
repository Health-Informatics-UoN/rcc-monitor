export interface StudyPartial {
  id: string;
  name: string;
}

export interface Study extends StudyPartial {
  apiKey: string;
}

export interface FormState {
  message: string;
  success: boolean;
}
