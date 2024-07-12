"use client";

import { Form, Formik } from "formik";
import { AlertCircle, Plus } from "lucide-react";
import React from "react";
import { number, object, string } from "yup";

import { FormikInput } from "@/components/forms/FormikInput";
import { Study } from "@/types/studies";

import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { Button } from "../ui/button";
import { DialogFooter } from "../ui/dialog";

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
  handleSubmit: (
    values: { apiKey: string },
    helpers: { resetForm: () => void }
  ) => Promise<void>;
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
              <Plus className={`icon-md ml-2`} />
            </Button>
          </DialogFooter>

          {feedback && (
            <Alert variant="destructive" className={`mt-4`}>
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
