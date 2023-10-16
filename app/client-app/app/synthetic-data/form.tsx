"use client";

// @ts-ignore
import { experimental_useFormState as useFormState } from "react-dom";
import { experimental_useFormStatus as useFormStatus } from "react-dom";

import { postSpreadsheet } from "@/lib/api/syntheticdata";

import { Button, ButtonProps } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FileDown, UploadCloud, XCircle } from "lucide-react";
import { Label } from "@/components/ui/label";
import { css } from "@/styled-system/css";
import { hstack } from "@/styled-system/patterns";

const initialState = {
  message: null,
};

interface ValidatedButtonProps extends ButtonProps {
  state: string;
}

function ValidatedButton({ state }: ValidatedButtonProps) {
  const success: boolean = state === "success";

  return (
    <Button
      variant="outline"
      color={success ? "green.600" : "red.600"}
      border="2px solid"
      borderColor={success ? "green.600" : "red.600"}
      h="55px"
    >
      {success ? <FileDown color="green" /> : <XCircle color="red" />}
      {success ? "Download spreadsheet" : "Validation failed"}
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
    <form
      action={formAction}
      className={css({
        spaceY: "4",
      })}
    >
      <Label htmlFor="event">Event Name</Label>
      <Input name="eventName" type="text" required min={3} />

      <Label htmlFor="file">Enter File</Label>
      <Input name="file" type="file" required />

      <div className={hstack({ gap: "6" })}>
        <SubmitButton />
        {state.message && <ValidatedButton state={state?.message} />}
      </div>
    </form>
  );
}
