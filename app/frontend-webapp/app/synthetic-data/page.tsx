import { Metadata } from "next";

import { css } from "@/styled-system/css";
import { Box, Flex } from "@/styled-system/jsx";
import { AlertCircle } from "lucide-react";
import { grid } from "@/styled-system/patterns";

import { Alert, AlertTitle } from "@/components/shadow-ui/Alert";
import { UploadFile } from "./form";
import { h1 } from "@/styled-system/recipes";

export const metadata: Metadata = {
  title: "RedCap Synthetic Data",
};

export default async function Page() {
  return (
    <div
      className={grid({
        columns: { base: 1, md: 2 },
        gap: "6",
        lineHeight: "2rem",
      })}
    >
      <Box>
        <h1 className={h1()}>Synthetic Data Generation</h1>
        <p>
          A tool that will generate synthetic data if you upload a RedCap Cloud
          data dictionary. This data is suitable for adding subjects to a study
          event to perform study changes and migrations.
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
          <li>Test the data dictionary.</li>
          <li>Generate realistic synthetic data.</li>
          <li>Respect branching logic (this is handled by RedCap on import)</li>
          <li>
            Populate calculated fields (this is handled by RedCap on import).
          </li>
          <li>Understand fields beyond the field type.</li>
        </ol>

        <h2
          className={css({
            fontSize: "1.5rem",
            fontWeight: "bold",
            m: "20px 0px",
          })}
        >
          How to use it
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
      </Box>

      <Flex direction={"column"} justifyContent={"center"} gap={"6"}>
        <UploadFile />
        <Alert backgroundColor="blue.200">
          <AlertCircle />
          <AlertTitle>
            You can upload a data dictionary in a supported format: .csv
          </AlertTitle>
        </Alert>
      </Flex>
    </div>
  );
}
