"use client";

import React from "react";
import { AlertCircle, ChevronRight } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { FormState } from "@/types/studies";

import { Grid } from "@/styled-system/jsx";
import { icon } from "@/styled-system/recipes";

import { Submit } from "./submit";

export function ValidateForm({
  action,
  state,
}: {
  action: (a: FormData) => void;
  state: FormState;
}) {
  return (
    <form action={action}>
      <Grid gap="4" py="4">
        <Grid gridTemplateColumns="4" alignItems="center" gap="4">
          <Label htmlFor="apiKey" textAlign="right">
            Api Key
          </Label>
          <Input name="apiKey" id="apiKey" gridColumn="3" required min={"10"} />
        </Grid>
      </Grid>

      <DialogFooter>
        <Submit>
          Validate <ChevronRight className={icon({ right: "sm" })} />
        </Submit>
      </DialogFooter>
      {state?.message && (
        <Alert variant="destructive" mt={"4"}>
          <AlertCircle />
          <AlertTitle>Validation failed.</AlertTitle>
          <AlertDescription>{state.message}</AlertDescription>
        </Alert>
      )}
    </form>
  );
}
