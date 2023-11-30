import { object, string } from "yup";

export const validationSchema = object({
  eventName: string().required("Event name is required"),
});
