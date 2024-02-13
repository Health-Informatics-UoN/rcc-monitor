"use client";

import { Form, Formik } from "formik";
import { AlertCircle, FileDown, UploadCloud } from "lucide-react";
import Link from "next/link";
import React from "react";

import { postSpreadsheet } from "@/api/syntheticdata";
import { validationSchema } from "@/app/synthetic-data/validation";
import { FormikInput } from "@/components/forms/FormikInput";
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/components/shadow-ui/Alert";
import { Button } from "@/components/shadow-ui/Button";
import { toast } from "@/components/shadow-ui/Toast/use-toast";
import { ApiError } from "@/lib/api/error";
import { css } from "@/styled-system/css";
import { hstack } from "@/styled-system/patterns";
import { icon } from "@/styled-system/recipes";

function DownloadButton({ file }: { file: string }) {
  return (
    <Button
      variant="outline"
      color="green.600"
      border="2px solid"
      borderColor="green.600"
    >
      <Link href={`/api/synthetic-data/${file}`} download>
        Download Subject Data
      </Link>
      <FileDown color="green" />
    </Button>
  );
}

export function UploadFile() {
  // Formik needs the file to be set in state.
  const [file, setFile] = React.useState<File>();
  const [fileName, setFileName] = React.useState<string>();
  const [feedback, setFeedback] = React.useState<string>();

  async function handleSubmit(values: { eventName: string }) {
    if (!file || !values.eventName) {
      setFeedback("Select a .csv file.");
      return;
    } else {
      if (file.type != "text/csv") {
        setFeedback("File is not a .csv");
        return;
      }
    }
    const form = new FormData();
    form.append("file", file);

    for (const [k, v] of Object.entries(values)) {
      if (Array.isArray(v) && v) {
        for (let i = 0; i < v.length; i++) {
          form.append(`${k}[]`, v[i]);
        }
      } else form.append(k, v);
    }

    try {
      const response = await postSpreadsheet(form);
      setFileName(response.name);
      toast({
        title: "Synthetic data generated.",
      });
    } catch (e) {
      console.error(e);

      let message;
      if (e instanceof ApiError) message = e.message;
      else message = String(e);

      setFeedback(message);
      toast({
        variant: "destructive",
        title: "Failed to generate data.",
      });
    }
  }

  const handleSelectFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files) {
      setFile(event.target.files[0]);
    }
  };

  return (
    <Formik
      onSubmit={handleSubmit}
      initialValues={{ eventName: "", file: undefined }}
      validationSchema={validationSchema}
    >
      {({ isSubmitting }) => (
        <Form
          className={css({
            spaceY: "2",
          })}
          noValidate
        >
          <FormikInput
            name="eventName"
            label="RedCap Event Name"
            id="eventName"
            placeholder="Event Name"
            required
          />

          <FormikInput
            name="file"
            label="Select File"
            id="file"
            type="file"
            accept=".csv"
            onChange={handleSelectFile}
            required
          />

          <div className={hstack({ gap: "6" })}>
            <Button type="submit" disabled={isSubmitting}>
              Upload File
              <UploadCloud className={icon({ right: "sm" })} />
            </Button>
            {fileName && <DownloadButton file={fileName} />}
          </div>

          {feedback && (
            <Alert variant="destructive" mt={"4"}>
              <AlertCircle />
              <AlertTitle>Data generation failed.</AlertTitle>
              <AlertDescription>{feedback}</AlertDescription>
            </Alert>
          )}
        </Form>
      )}
    </Formik>
  );
}
