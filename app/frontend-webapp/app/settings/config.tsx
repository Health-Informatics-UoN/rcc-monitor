"use client";
import { Box, Flex, Grid } from "@/styled-system/jsx";
import { css } from "@/styled-system/css";
import { Input } from "@/components/shadow-ui/Input";
import { FileEdit } from "lucide-react";
import { useState } from "react";
import { ConfigModel, UpdateConfigModel } from "@/types/config";
import { Form, Formik } from "formik";
import { Button } from "@/components/shadow-ui/Button";
import { FormikInput } from "@/components/forms/FormikInput";
import { validationSchema } from "./validation";
import { updateSiteConfig } from "@/api/config";
import { toast } from "@/components/shadow-ui/Toast/use-toast";
import { Description } from "@/components/shared/Description";

export default function Config({ config }: { config: ConfigModel }) {
  const [update, setUpdate] = useState<boolean>(false);

  async function handleSubmit(values: UpdateConfigModel) {
    try {
      await updateSiteConfig(values);
      toast({
        title: "Setting updated.",
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
      <Grid gridTemplateColumns="1fr 1fr" m="25px 0px">
        <p
          className={css({
            fontSize: "18px",
            fontWeight: "700",
            m: "auto 0px",
          })}
        >
          {config.name}
        </p>
        {!update ? (
          <Flex gap="20px">
            <Input
              w="100%"
              border="2px solid"
              value={config?.value}
              fontSize="18px"
              readOnly
            />
            <div className={css({ m: "auto 0px" })}>
              <FileEdit onClick={() => setUpdate(true)} />
            </div>
          </Flex>
        ) : (
          <Box maxH="45px">
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
                  <Flex gap={2}>
                    <Box>
                      <FormikInput
                        name="value"
                        id="value"
                        type="text"
                        required
                      />
                    </Box>
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
                  </Flex>
                </Form>
              )}
            </Formik>
          </Box>
        )}
      </Grid>
    </Description>
  );
}
