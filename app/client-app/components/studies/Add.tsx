"use client";

// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
import { useFormState } from "react-dom";
import React, { useEffect } from "react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

import { Plus } from "lucide-react";

import { icon } from "@/styled-system/recipes";

import { addStudy, validateStudy } from "@/lib/api/studies";

import { CreateForm } from "./Create";
import { ValidateForm } from "./Validate";

export default function AddStudy() {
  const [validatedState, validate] = useFormState(validateStudy, null);
  const [createdState, create] = useFormState(addStudy, { success: null });
  const [open, setOpen] = React.useState(false);

  const validated = validatedState?.success;
  useEffect(() => {
    if (createdState?.success) {
      setOpen(false);
    }
  }, [createdState]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button>
          Add Study
          <Plus className={icon({ right: "sm" })} />
        </Button>
      </DialogTrigger>

      <DialogContent sm={{ maxW: "600px" }}>
        <DialogHeader>
          <DialogTitle>Add a new study</DialogTitle>
          <DialogDescription>
            Enter your RedCap API Key to get the study details.
            <br />
            Your key will be securely stored, and only used to monitor your
            study.
          </DialogDescription>
        </DialogHeader>

        {validated ? (
          <CreateForm
            action={create}
            state={createdState}
            model={validatedState?.study}
          />
        ) : (
          <ValidateForm action={validate} state={validatedState} />
        )}
      </DialogContent>
    </Dialog>
  );
}
