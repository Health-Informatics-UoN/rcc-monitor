"use client";

import React from "react";
import { AlertCircle, ChevronRight } from "lucide-react";
import { Formik, Form } from "formik";
import { object, string } from "yup";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { DialogFooter } from "@/components/ui/dialog";
import { icon } from "@/styled-system/recipes";

import { FormikInput } from "@/components/forms/FormikInput";
import { Button } from "@/components/ui/button";

const validationSchema = object({
  apiKey: string().required("API Key is required"),
});

export function ValidateForm({
  handleSubmit,
  feedback,
}: {
  handleSubmit: (values: { apiKey: string }) => Promise<void>;
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
              <ChevronRight className={icon({ right: "sm" })} />
            </Button>
          </DialogFooter>
          {feedback && (
            <Alert variant="destructive" mt={"4"}>
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
