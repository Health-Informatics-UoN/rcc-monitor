"use client";

import { formatDistanceToNow } from "date-fns";
import { Form, Formik } from "formik";
import {
  AlertCircle,
  AlertTriangle,
  BellOff,
  BellRing,
  Plus,
} from "lucide-react";
import React from "react";
import { number, object, string } from "yup";

import { updateStudy } from "@/api/studies";
import { FormikInput } from "@/components/forms/FormikInput";
import { Description } from "@/components/shared/Description";
import EnvironmentBadge from "@/components/shared/EnvironmentBadge";
import { UserManagement } from "@/components/studies/UserManagement";
import { configKeys } from "@/constants/configKeys";
import { ConfigModel } from "@/types/config";
import { StudyPartial } from "@/types/studies";

import { Alert, AlertDescription, AlertTitle } from "../ui/alert";
import { Button } from "../ui/button";
import { Separator } from "../ui/separator";
import { Switch } from "../ui/switch";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "../ui/table";
import { useToast } from "../ui/use-toast";

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
  const { toast } = useToast();
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
          <h1 className={`h1`}>{model.name}</h1>
          <div className="flex gap-5 my-5 mx-0 h-5 items-end">
            <div>
              <p className="font-bold">
                RedCap Id: <span>{model.id}</span>
              </p>
            </div>
            <EnvironmentBadge name={model.instance} />
          </div>
          <div className="grid gap-4 py-4">
            <Separator />
            <h4 className={`h4`}>Users</h4>
            <UserManagement users={values.users} />
          </div>

          {model.studyGroup?.length > 0 && (
            <div className="my-5 mx-0">
              <h4 className={`h4`}>Study Groups</h4>
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
            </div>
          )}
          <Separator />

          <div className="grid gap-4 py-4">
            <h4 className={`h4`}>Subjects Enrolled</h4>
            {model.instance === "Build" &&
              model.subjectsEnrolled > model.subjectsEnrolledThreshold && (
                <div className="w-[80%]">
                  <Alert variant="destructive">
                    <AlertTriangle className={`icon-md`} />
                    <AlertTitle>Subjects enrolled capacity exceeded</AlertTitle>
                    <AlertDescription>
                      The capacity is {model.subjectsEnrolledThreshold} and you
                      currently have {model.subjectsEnrolled} subjects enrolled
                    </AlertDescription>
                  </Alert>
                </div>
              )}
            <div className="flex gap-5">
              <div>
                <p className="font-bold">
                  Capacity: <span>{model.subjectsEnrolledThreshold}</span>
                </p>
              </div>
              <div>
                <p className="font-bold">
                  Subjects Enrolled: <span>{model.subjectsEnrolled}</span>
                </p>
              </div>
            </div>
          </div>

          <div className="my-[50px] mx-0">
            <div className="flex gap-3 items-center">
              <h4 className={`h4`}>Study Capacity</h4>
              {model.studyCapacityAlert ? (
                <BellRing className={`icon-md`} />
              ) : (
                <BellOff className={`icon-md`} />
              )}
            </div>

            <div className="my-[30px] mx-0">
              <p className="font-bold">
                Last Checked:{" "}
                <span className="font-normal">
                  {formatDistanceToNow(
                    new Date(model.studyCapacityLastChecked),
                    {
                      addSuffix: true,
                    }
                  )}
                </span>
              </p>
            </div>
            <div>
              <div className="flex gap-5 items-center mt-[15px] mb-[25px] mx-0">
                <p className="font-bold">Alerts</p>
                <Switch
                  checked={isRandChecked}
                  onCheckedChange={() =>
                    handleChecked(isRandChecked, setRandChecked)
                  }
                />
              </div>
              <div className="w-[200px]">
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
              </div>
            </div>
          </div>

          <div className="flex justify-end">
            <Button type="submit" disabled={isSubmitting}>
              Update
              <Plus className={`icon-md ml-2`} />
            </Button>
          </div>

          {feedback && (
            <Alert variant="destructive" className={`mt-4`}>
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
