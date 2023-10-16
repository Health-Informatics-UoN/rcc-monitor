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
          Synthetic Data Generation
        </h1>
        A tool that will generate synthetic data if you upload a RedCap Cloud
        data dictionary. This data is suitable for adding subjects to a study to
        perform study changes and migrations. It does not test studies or
        provide realistic synthetic data.
        <h2>How to use</h2>
        <ol>
          <li>
            Download your RedCap data dictionary from RedCap cloud. Go to your
            study, the Instruments tab, the tools dropdown, and Export Data
            Dictionary.
          </li>
          <li>
            Select the instruments you would like to download the dictionaries
            for.
          </li>
          <li>
            Upload your file here, input the event name you wish to generate
            data for, this should match the event name in RedCap.
          </li>
          <li>Download the file when it has generated.</li>
          <li>
            Upload the synthetic data to RedCap. Go to your study, and select
            Data in the left menu, select Data Import Tool in the tabs.
          </li>
          <li>
            Complete the form as necessary (the date format is the default), and
            choose the file as your upload.
          </li>
          <li>
            RedCap will process the data, and display the data it will import to
            you, select through the options to import the data.
          </li>
          <li>
            The synthetic data should now have populated your subjects data.
          </li>
        </ol>
        <p>
          {`The tool will generate data based on the type of field and the minimum and maximum validation criteria. 
          It will not:
          respect branching
          populate calculated fields
          Understand fields beyong the " - for example, names, genders, addresses.
          `}
        </p>
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
