import { object, string } from "yup";

export const validationSchema = object({
  value: string().required("Fill in a valid value"),
});
