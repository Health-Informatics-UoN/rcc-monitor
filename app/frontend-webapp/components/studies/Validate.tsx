"use client";

import { Form, Formik } from "formik";
import { AlertCircle, ChevronRight } from "lucide-react";
import React from "react";
import { object, string } from "yup";

import { FormikInput } from "@/components/forms/FormikInput";

import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { Button } from "../ui/button";
import { DialogFooter } from "../ui/dialog";

const validationSchema = object({
  apiKey: string().required("API Key is required"),
});

export function ValidateForm({
  handleSubmit,
  feedback,
}: {
  handleSubmit: (
    values: { apiKey: string },
    helpers: { resetForm: () => void }
  ) => Promise<void>;
  feedback: string | undefined;
}) {
  return (
    <Formik
      onSubmit={handleSubmit}
      initialValues={{ apiKey: "" }}
      validationSchema={validationSchema}
    >
      {({ isSubmitting }) => (
        <Form noValidate>
          <FormikInput name="apiKey" id="apiKey" label="API Key" required />

          <DialogFooter>
            <Button type="submit" disabled={isSubmitting}>
              Validate
              <ChevronRight className={`icon-md ml-2`} />
            </Button>
          </DialogFooter>
          {feedback && (
            <Alert variant="destructive" className={`mt-4`}>
              <AlertCircle />
              <AlertTitle>Validation failed.</AlertTitle>
              <AlertDescription>{feedback}</AlertDescription>
            </Alert>
          )}
        </Form>
      )}
    </Formik>
  );
}
