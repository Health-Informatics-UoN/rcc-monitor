"use client";

import React from "react";
import { Formik, Field, Form } from "formik";
import { AlertCircle, Plus } from "lucide-react";
import { css } from "@/styled-system/css";
import { Grid } from "@/styled-system/jsx";
import { icon } from "@/styled-system/recipes";

import { UserManagement } from "@/components/studies/UserManagement";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";

import { StudyPartial } from "@/types/studies";
import { updateStudy } from "@/lib/api/studies";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast/use-toast";

export function UpdateForm({ model }: { model: StudyPartial }) {
  const [feedback, setFeedback] = React.useState<string>();

  async function handleSubmit(values: StudyPartial) {
    try {
      await updateStudy(values);
      toast({
        title: "Study updated.",
      });
    } catch (e) {
      console.error(e);
      setFeedback("Error");
      toast({
        variant: "destructive",
        title: "Failed to updated study.",
      });
    }
  }

  return (
    <Formik onSubmit={handleSubmit} initialValues={model}>
      {({ isSubmitting, values }) => (
        <Form noValidate>
          <h2
            className={css({
              fontSize: "20px",
              fontWeight: "bold",
            })}
          >
            Study Details
          </h2>
          <Grid gap="4" py="4">
            <Grid gridTemplateColumns="4" alignItems="center" gap="4">
              <Label htmlFor="id" textAlign="right">
                Study ID
              </Label>
              <Field
                component={Input}
                name="Id"
                id="Id"
                value={values.id}
                gridColumn="3"
                color={"gray.400"}
                border={"gray.400"}
                readOnly
              />

              <Label htmlFor="Name" textAlign="right">
                Study Name
              </Label>
              <Field
                component={Input}
                name="Name"
                id="Name"
                value={values.name}
                gridColumn="3"
                color={"gray.400"}
                border={"gray.400"}
                readOnly
              />
            </Grid>
            <Separator />
            <h2
              className={css({
                fontSize: "20px",
                fontWeight: "bold",
              })}
            >
              Users
            </h2>
            <UserManagement users={values.users} />
          </Grid>

          <div
            className={css({
              display: "flex",
              justifyContent: "flex-end",
            })}
          >
            <Button type="submit" disabled={isSubmitting}>
              Update
              <Plus className={icon({ right: "sm" })} />
            </Button>
          </div>

          {feedback && (
            <Alert variant="destructive" mt={"4"}>
              <AlertCircle />
              <AlertTitle>Updating study failed.</AlertTitle>
              <AlertDescription>{feedback}</AlertDescription>
            </Alert>
          )}
        </Form>
      )}
    </Formik>
  );
}
