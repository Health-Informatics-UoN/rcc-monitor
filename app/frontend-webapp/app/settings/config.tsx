"use client";
import { Form, Formik } from "formik";
import { FileEdit } from "lucide-react";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useToast } from "@/components/ui/use-toast";
import { ConfigModel, UpdateConfigModel } from "@/types/config";

import { updateSiteConfig } from "../../api/config";
import { FormikInput } from "../../components/forms/FormikInput";
import { Description } from "../../components/shared/Description";
import { validationSchema } from "./validation";

export default function Config({ config }: { config: ConfigModel }) {
  const { toast } = useToast();
  const [update, setUpdate] = useState<boolean>(false);

  async function handleSubmit(values: UpdateConfigModel) {
    try {
      await updateSiteConfig(values);
      toast({
        variant: "success",
        title: "Setting updated",
      });
      setUpdate(false);
    } catch (error) {
      console.error(error);
      let errorMessage;
      if (error instanceof Error) {
        errorMessage = error.message;
      }
      toast({
        variant: "destructive",
        title: "Failed to update settings.",
        description: errorMessage,
      });
    }
  }

  return (
    <Description text={config.description}>
      <div className="grid grid-cols-2 my-6">
        <p className="text-lg font-bold my-auto mx-0">{config.name}</p>
        {!update ? (
          <div className="flex gap-5">
            <Input
              className="w-full border-2 text-lg"
              value={config?.value}
              readOnly
            />
            <div className="my-auto mx-0">
              <FileEdit
                className="cursor-pointer"
                onClick={() => setUpdate(true)}
              />
            </div>
          </div>
        ) : (
          <div className="max-h-11">
            <Formik
              onSubmit={handleSubmit}
              initialValues={{
                key: config ? config.key : "",
                value: config ? config.value : "",
              }}
              validationSchema={validationSchema}
            >
              {({ isSubmitting }) => (
                <Form noValidate>
                  <div className="flex gap-2">
                    <div>
                      <FormikInput
                        name="value"
                        id="value"
                        type="text"
                        required
                      />
                    </div>
                    <Button size="sm" type="submit" disabled={isSubmitting}>
                      Save
                    </Button>
                    <Button
                      size="sm"
                      type="button"
                      variant="destructive"
                      disabled={isSubmitting}
                      onClick={() => setUpdate(false)}
                    >
                      Cancel
                    </Button>
                  </div>
                </Form>
              )}
            </Formik>
          </div>
        )}
      </div>
    </Description>
  );
}
