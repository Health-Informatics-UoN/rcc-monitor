import { Metadata } from "next";
import { AddForm } from "./form";
import { css } from "@/styled-system/css";
import { Box, Flex } from "@/styled-system/jsx";
import { Alert, AlertTitle } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";

export const metadata: Metadata = {
  title: "RedCap Synthetic Data",
};

export default async function Page() {
  return (
    <Flex h="40vh">
      <Box m="auto">
        <h1
          className={css({
            fontSize: "2rem",
            fontWeight: "bold",
            m: "20px 0px",
          })}
        >
          Upload a spreadsheet to generate data
        </h1>
        <Alert backgroundColor="blue.200" mb="10px">
          <AlertCircle />
          <AlertTitle>
            You can upload a spreadsheet in a supported format: .csv
          </AlertTitle>
        </Alert>
        <AddForm />
      </Box>
    </Flex>
  );
}
