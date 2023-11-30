"use client";

import React from "react";
import { Formik, Form } from "formik";
import { object, string, number } from "yup";
import { AlertCircle, Plus } from "lucide-react";
import { icon } from "@/styled-system/recipes";

import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/components/shadow-ui/Alert";
import { DialogFooter } from "@/components/shadow-ui/Dialog";
import { FormikInput } from "@/components/forms/FormikInput";
import { Button } from "@/components/shadow-ui/Button";

import { Study } from "@/types/studies";

const validationSchema = object({
  name: string().required(),
  id: number().required(),
  apiKey: string().required(),
  instance: string().required(),
});

// This is a readonly form for the user to check the values, not edit them.
export function CreateForm({
  handleSubmit,
  feedback,
  model,
}: {
  handleSubmit: (values: { apiKey: string }) => Promise<void>;
  feedback: string | undefined;
  model: Study;
}) {
  return (
    <Formik
      onSubmit={handleSubmit}
      initialValues={model}
      validationSchema={validationSchema}
    >
      {({ isSubmitting }) => (
        <Form noValidate>
          <FormikInput name="apiKey" id="apiKey" label="API Key" disabled />
          <FormikInput name="id" id="id" label="Study Id" disabled />
          <FormikInput
            name="instance"
            id="instance"
            label="RedCap Environment"
            disabled
          />
          <FormikInput name="name" id="name" label="Study Name" disabled />

          <DialogFooter>
            <Button type="submit" disabled={isSubmitting}>
              Add
              <Plus className={icon({ right: "sm" })} />
            </Button>
          </DialogFooter>

          {feedback && (
            <Alert variant="destructive" mt={"4"}>
              <AlertCircle />
              <AlertTitle>Adding study failed.</AlertTitle>
              <AlertDescription>{feedback}</AlertDescription>
            </Alert>
          )}
        </Form>
      )}
    </Formik>
  );
}
