"use client";

import React from "react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { AlertCircle, Plus } from "lucide-react";
import { Grid } from "@/styled-system/jsx";
import { icon } from "@/styled-system/recipes";

import { FormState, Study } from "@/types/studies";
import { Submit } from "./submit";

export function CreateForm({
  action,
  state,
  model,
}: {
  action: (a: FormData) => void;
  state: FormState;
  model: Study;
}) {
  return (
    <form action={action}>
      <Grid gap="4" py="4">
        <Grid gridTemplateColumns="4" alignItems="center" gap="4">
          <Label htmlFor="apiKey" textAlign="right">
            Api Key
          </Label>
          <Input
            name="apiKey"
            id="apiKey"
            value={model.apiKey}
            gridColumn="3"
            color={"gray.400"}
            border={"gray.400"}
            readOnly
          />

          <Label htmlFor="id" textAlign="right">
            Study ID
          </Label>
          <Input
            name="Id"
            id="Id"
            value={model.id}
            gridColumn="3"
            color={"gray.400"}
            border={"gray.400"}
            readOnly
          />

          <Label htmlFor="Name" textAlign="right">
            Study Name
          </Label>
          <Input
            name="Name"
            id="Name"
            value={model.name}
            gridColumn="3"
            color={"gray.400"}
            border={"gray.400"}
            readOnly
          />
        </Grid>
      </Grid>

      <DialogFooter>
        <Submit>
          Add <Plus className={icon({ right: "sm" })} />
        </Submit>
      </DialogFooter>

      {state?.message && (
        <Alert variant="destructive" mt={"4"}>
          <AlertCircle />
          <AlertTitle>Adding study failed.</AlertTitle>
          <AlertDescription>{state.message}</AlertDescription>
        </Alert>
      )}
    </form>
  );
}
