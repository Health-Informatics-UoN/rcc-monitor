"use client";

// @ts-ignore
import { experimental_useFormState as useFormState } from "react-dom";
import { experimental_useFormStatus as useFormStatus } from "react-dom";
import { postSpreadsheet } from "@/lib/api/syntheticdata";

const initialState = {
  message: null,
};

function SubmitButton() {
  const { pending } = useFormStatus();

  return (
    <button type="submit" aria-disabled={pending}>
      Upload
    </button>
  );
}

export function AddForm() {
  const [state, formAction] = useFormState(postSpreadsheet, initialState);

  return (
    <form action={formAction}>
      <label htmlFor="todo">Enter File</label>
      <input type="file" name="file" required />
      <SubmitButton />
      <p aria-live="polite" className="sr-only">
        {state?.message}
      </p>
    </form>
  );
}
