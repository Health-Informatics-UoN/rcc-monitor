"use client";

// @ts-ignore
import { experimental_useFormState as useFormState } from "react-dom";
import { experimental_useFormStatus as useFormStatus } from "react-dom";
import Link from "next/link";
import { FileDown, UploadCloud, XCircle } from "lucide-react";

import { postSpreadsheet } from "@/lib/api/syntheticdata";

import { css } from "@/styled-system/css";
import { hstack } from "@/styled-system/patterns";
import { Button, ButtonProps } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

const initialState = {
  message: null,
};

interface ValidatedButtonProps extends ButtonProps {
  state: {
    message: string;
    url: string;
  };
}

function ValidatedButton({ state }: ValidatedButtonProps) {
  const success: boolean = state?.message === "success";

  return (
    <Button
      variant="outline"
      color={success ? "green.600" : "red.600"}
      border="2px solid"
      borderColor={success ? "green.600" : "red.600"}
      h="55px"
    >
      {success ? <FileDown color="green" /> : <XCircle color="red" />}
      {success ? (
        <Link href={state.url}>Download Spreadsheet</Link>
      ) : (
        "Validation Failed"
      )}
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

export function UploadFile() {
  const [state, formAction] = useFormState(postSpreadsheet, initialState);

  return (
    <form
      action={formAction}
      className={css({
        spaceY: "2",
      })}
    >
      <Label htmlFor="event">RedCap Event Name</Label>
      <Input
        name="eventName"
        type="text"
        required
        min={3}
        placeholder="Event Name"
      />

      <Label htmlFor="file">Select File</Label>
      <Input name="file" type="file" required />

      <div className={hstack({ gap: "6" })}>
        <SubmitButton />
        {state.message && <ValidatedButton state={state} />}
      </div>
    </form>
  );
}
