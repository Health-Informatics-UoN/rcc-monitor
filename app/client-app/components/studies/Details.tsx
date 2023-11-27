"use client";

import React from "react";
import { Formik, Form } from "formik";
import { AlertCircle, BellOff, BellRing, Plus } from "lucide-react";
import { css } from "@/styled-system/css";
import { Box, Flex, Grid } from "@/styled-system/jsx";
import { h1, h4, icon } from "@/styled-system/recipes";

import { UserManagement } from "@/components/studies/UserManagement";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Separator } from "@/components/ui/separator";

import { StudyPartial } from "@/types/studies";
import { updateStudy } from "@/lib/api/studies";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/toast/use-toast";
import { FormikInput } from "@/components/forms/FormikInput";
import { Switch } from "@/components/ui/switch";
import { object, string, number } from "yup";
import { Description } from "@/components/Description";
import { ConfigModel } from "@/types/config";
import { configKeys } from "@/constants/configKeys";
import EnvironmentBadge from "@/components/EnvironmentBadge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Alert as CustomAlert } from "@/components/Alert";

const validationSchema = object({
  studyCapacityThreshold: number()
    .typeError("Must be a number")
    .required("Fill in a valid value")
    .min(0, "Must not be less than zero")
    .max(100, "Must not be greater than 100"),
  studyCapacityJobFrequency: string().required("Fill in a valid value"),
});

interface UpdateFormProps {
  model: StudyPartial;
  config: ConfigModel[];
}

export function DetailsPage({ model, config }: UpdateFormProps) {
  const [feedback, setFeedback] = React.useState<string>();
  const [isRandChecked, setRandChecked] = React.useState<boolean>(false);
  const thresholdDescription =
    config.find((x) => x.key == configKeys.RandomisationThreshold)
      ?.description ?? "";
  const frequencyDescription =
    config.find((x) => x.key == configKeys.RandomisationJobFrequency)
      ?.description ?? "";

  const handleChecked = (
    checked: boolean,
    setChecked: (val: boolean) => void
  ) => {
    if (!checked) {
      setChecked(true);
    } else {
      setChecked(false);
    }
  };

  React.useEffect(() => {
    if (model.studyCapacityAlertsActivated) {
      setRandChecked(true);
    }
  }, []);

  async function handleSubmit(values: StudyPartial) {
    try {
      await updateStudy({
        ...values,
        studyCapacityAlertsActivated: isRandChecked,
        studyCapacityThreshold: values.studyCapacityThreshold / 100,
      });
      toast({
        title: "Study updated.",
      });
    } catch (e) {
      console.error(e);
      let errorMessage;
      if (e instanceof Error) errorMessage = e.message;
      setFeedback("Error");
      toast({
        variant: "destructive",
        title: "Failed to updated study.",
        description: errorMessage,
      });
    }
  }

  return (
    <Formik
      onSubmit={handleSubmit}
      initialValues={{
        ...model,
        studyCapacityThreshold: model.studyCapacityThreshold * 100,
      }}
      validationSchema={validationSchema}
    >
      {({ isSubmitting, values }) => (
        <Form noValidate>
          <h1 className={h1()}>{model.name}</h1>
          <Flex gap={5} m="20px 0" h="20px" alignItems="end">
            <Box>
              <p className={css({ fontWeight: "bold" })}>
                RedCap Id: <span>{model.id}</span>
              </p>
            </Box>
            <EnvironmentBadge name={model.instance} />
          </Flex>
          <Grid gap="4" py="4">
            <Separator />
            <h4 className={h4()}>Users</h4>
            <UserManagement users={values.users} />
          </Grid>

          <Box m="20px 0">
            <h4 className={h4()}>Study Groups</h4>
            {model.studyGroup?.length > 0 ? (
              <Table>
                <TableHeader>
                  <TableHead>Name</TableHead>
                  <TableHead>Planned Size</TableHead>
                </TableHeader>
                <TableBody>
                  {model.studyGroup.map((group) => (
                    <TableRow key={group.id}>
                      <TableCell>{group.name}</TableCell>
                      <TableCell>{group.plannedSize}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <Box m="10px 0">
                <CustomAlert message="There are no study groups" />
              </Box>
            )}
          </Box>

          <Separator />

          <div className={css({ m: "50px 0px" })}>
            <Flex gap={3} alignItems="center">
              <h4 className={h4()}>Study Capacity</h4>
              {model.studyCapacityAlert ? (
                <BellRing className={icon()} />
              ) : (
                <BellOff className={icon()} />
              )}
            </Flex>

            <Box m="30px 0">
              <p className={css({ fontWeight: "bold" })}>
                Last Checked:{" "}
                <span className={css({ fontWeight: "normal" })}>
                  {model.studyCapacityLastChecked}
                </span>
              </p>
            </Box>
            <div>
              <Flex gap={5} alignItems="center" m="15px 0px 25px 0px">
                <p className={css({ fontWeight: "bold" })}>Alerts</p>
                <Switch
                  checked={isRandChecked}
                  onCheckedChange={() =>
                    handleChecked(isRandChecked, setRandChecked)
                  }
                />
              </Flex>
              <Box w="200px">
                <Description text={thresholdDescription}>
                  <div>
                    <FormikInput
                      name="studyCapacityThreshold"
                      id="studyCapacityThreshold"
                      label="Threshold (%)"
                      disabled={!isRandChecked}
                    />
                  </div>
                </Description>
                <Description text={frequencyDescription}>
                  <div>
                    <FormikInput
                      name="studyCapacityJobFrequency"
                      id="Study Capacity Job Frequency"
                      label="Frequency"
                      disabled={!isRandChecked}
                    />
                  </div>
                </Description>
              </Box>
            </div>
          </div>

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
