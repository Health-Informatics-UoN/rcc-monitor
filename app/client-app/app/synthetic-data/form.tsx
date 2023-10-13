"use client";

// @ts-ignore
import { experimental_useFormState as useFormState } from "react-dom";
import { experimental_useFormStatus as useFormStatus } from "react-dom";

import { postSpreadsheet } from "@/lib/api/syntheticdata";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { AlertCircle, FileDown, UploadCloud, XCircle } from "lucide-react";

const initialState = {
  message: null,
};

interface ValidatedButtonProps {
  variant: string;
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
}

function ValidatedButton({ variant, onClick }: ValidatedButtonProps) {
  const type: boolean = variant === "success";

  return (
    <Button
      variant="outline"
      color={type ? "green.600" : "red.600"}
      border="2px solid"
      borderColor={type ? "green.600" : "red.600"}
      h="55px"
      onClick={onClick}
    >
      {type ? <FileDown color="green" /> : <XCircle color="red" />}
      {type ? "Download spreadsheet" : "Validation failed"}
    </Button>
  );
}

function SubmitButton() {
  const { pending } = useFormStatus();

  return (
    <Button
      type="submit"
      aria-disabled={pending}
      backgroundColor="#0074d9"
      h="55px"
      _hover={{
        backgroundColor: "#56a1d1",
        borderColor: "#56a1d1",
      }}
    >
      <UploadCloud />
      Upload File
    </Button>
  );
}

export function AddForm() {
  const [state, formAction] = useFormState(postSpreadsheet, initialState);

  return (
    <form action={formAction}>
      <label htmlFor="file">Enter File</label>
      <Input name="file" type="file" required />
      <SubmitButton />
      {state.message && <ValidatedButton variant={state?.message} />}
    </form>
  );
}
