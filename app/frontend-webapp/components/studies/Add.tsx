"use client";

import { Plus } from "lucide-react";
import React from "react";

import { addStudy, validateStudy } from "@/api/studies";
import { Button } from "@/components/shadow-ui/Button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/shadow-ui/Dialog";
import { toast } from "@/components/shadow-ui/Toast/use-toast";
import { ApiError } from "@/lib/api/error";
import { icon } from "@/styled-system/recipes";
import { Study } from "@/types/studies";

import { CreateForm } from "./Create";
import { ValidateForm } from "./Validate";

export default function AddStudy() {
  const [validatedFeedback, setValidatedFeedback] = React.useState<string>();
  const [createFeedback, setCreateFeedback] = React.useState<string>();
  const [study, setStudy] = React.useState<Study>();
  const [open, setOpen] = React.useState(false);

  async function handleValidate(
    values: { apiKey: string },
    { resetForm }: { resetForm: () => void }
  ) {
    try {
      const model = await validateStudy(values);
      setStudy(model);
    } catch (error) {
      console.error(error);

      let message;
      if (error instanceof ApiError) message = error.message;
      else message = String(error);

      setValidatedFeedback(message);
      toast({
        variant: "destructive",
        title: "Failed to validate study.",
      });
    }
    resetForm();
  }

  async function handleCreate(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    values: any,
    { resetForm }: { resetForm: () => void }
  ) {
    try {
      await addStudy(values);
      setOpen(false);
      toast({
        title: "Added new study.",
      });
    } catch (error) {
      console.error(error);

      let message;
      if (error instanceof ApiError) message = error.message;
      else message = String(error);

      setCreateFeedback(message);
      toast({
        variant: "destructive",
        title: "Failed to add study.",
      });
    }
    resetForm();
  }

  function handleOpenChange() {
    setOpen(!open);
    setValidatedFeedback(undefined);
    setCreateFeedback(undefined);
    setStudy(undefined);
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
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

        {study ? (
          <CreateForm
            handleSubmit={handleCreate}
            feedback={createFeedback}
            model={study}
          />
        ) : (
          <ValidateForm
            handleSubmit={handleValidate}
            feedback={validatedFeedback}
          />
        )}
      </DialogContent>
    </Dialog>
  );
}
