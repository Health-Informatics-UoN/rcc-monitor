import { Metadata } from "next";
import { AddForm } from "./form";
import { css } from "@/styled-system/css";
import { Box, VStack } from "@/styled-system/jsx";
import { Alert, AlertTitle } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";

export const metadata: Metadata = {
  title: "RedCap Synthetic Data",
};

export default async function Page() {
  return (
    <VStack maxWidth={"1/2"} lineHeight={"2rem"}>
      <Box m="auto" gap={"2"}>
        <h1
          className={css({
            fontSize: "2rem",
            fontWeight: "bold",
            m: "20px 0px",
          })}
        >
          Synthetic Data Generation
        </h1>
        <p>
          A tool that will generate synthetic data if you upload a RedCap Cloud
          data dictionary. This data is suitable for adding subjects to a study
          to perform study changes and migrations.
        </p>
        <p>
          The tool will generate data based on the type of field and the minimum
          and maximum validation criteria. It will not:
        </p>
        <ol
          className={css({
            listStyle: "disc",
          })}
        >
          <li>test the data dictionary</li>
          <li>generate realistic synthetic data</li>
          <li>respect branching logic</li>
          <li>populate calculated fields</li>
          <li>
            understand fields beyond the field type - for example names and
            addresses are treat the same as any other text.
          </li>
        </ol>

        <h2
          className={css({
            fontSize: "1.5rem",
            fontWeight: "bold",
            m: "20px 0px",
          })}
        >
          How to use
        </h2>
        <ol
          className={css({
            listStyleType: "decimal",
            listStyle: "decimal",
          })}
        >
          <li>
            Download your RedCap data dictionary from RedCap cloud. Go to your
            study, the Instruments tab, the Tools dropdown, and select Export
            Data Dictionary.
          </li>
          <li>
            Select the instruments you would like to generate the data for and
            download them.
          </li>
          <li>
            Upload the downloaded file to this tool, input the event name you
            wish to generate data for, this should match the event name in
            RedCap.
          </li>
          <li>When completed, download the file generated.</li>
          <li>
            Upload the synthetic data to RedCap: Go to your study, and select
            Data in the left menu, select Data Import Tool in the tabs.
          </li>
          <li>
            Complete the form as necessary (the date format is the default), and
            choose the downloaded synthetic data file as your upload.
          </li>
          <li>
            RedCap will process the data, and display the data it will import to
            you, select through the options to import the data.
          </li>
          <li>
            The synthetic data should now have populated your subjects data.
          </li>
        </ol>

        <Alert backgroundColor="blue.200" mb="10px">
          <AlertCircle />
          <AlertTitle>
            You can upload a spreadsheet in a supported format: .csv
          </AlertTitle>
        </Alert>
        <AddForm />
      </Box>
    </VStack>
  );
}
