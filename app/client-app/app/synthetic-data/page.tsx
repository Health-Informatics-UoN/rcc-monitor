"use client";
import { useRef, useState } from "react";
import { Button } from "@/components/ui/button";
import { css } from "@/styled-system/css";
import { Box, Flex } from "@/styled-system/jsx";
import { AlertCircle, FileDown, UploadCloud, XCircle } from "lucide-react";
import { useToast } from "@/components/ui/toast/use-toast";
import { Alert, AlertTitle } from "@/components/ui/alert";
import { postSpreadsheet } from "@/lib/api/actions";
import Spinner from "@/components/ui/spinner";

function ErrorText() {
  return (
    <p className={css({ color: "red", m: "-16px 0px 10px" })}>
      Please upload a valid spreadsheet
    </p>
  );
}

function ValidationText() {
  return (
    <Flex>
      <Spinner />
      <h1
        className={css({
          m: "auto",
          fontSize: "xl",
          fontStyle: "italic",
        })}
      >
        Validating...
      </h1>
    </Flex>
  );
}

function ValidatedButton({ variant }: { variant: string }) {
  const type: boolean = variant === "success";

  return (
    <Button
      type="button"
      size="lg"
      variant="outline"
      color={type ? "green.600" : "red.600"}
      m="10px 0px"
      fontWeight="bold"
      fontSize="lg"
      h="55px"
      border="2px solid"
      borderColor={type ? "green.600" : "red.600"}
    >
      {type ? <FileDown color="green" /> : <XCircle color="red" />}
      {type ? "Download spreadsheet" : "Validation failed"}
    </Button>
  );
}

export default function SyntheticData() {
  const { toast } = useToast();
  const ref = useRef<HTMLFormElement>(null);
  const [file, setFile] = useState<File | null>(null);
  const [spreadsheet, setSpreadsheet] = useState<File | null>(null);
  const [error, setError] = useState<boolean>(false);
  const [uploaded, setUploaded] = useState<boolean>(false);
  const [validated, setValidated] = useState<"success" | "fail" | "">("");

  async function generateData(formdata: FormData) {
    ref.current?.reset();

    if (file !== null) {
      setUploaded(true);
      try {
        const response = await postSpreadsheet(formdata);
        if (response) {
          setSpreadsheet(response);
          setUploaded(false);
          setValidated("success");
          toast({
            title: "Spreadsheet validation successful!",
            description: "Click on the button to download the file",
          });
        }
        setFile(null);
      } catch (error: any) {
        console.error(error);
        toast({
          variant: "destructive",
          title: "Spreadsheet validation failed!",
          description: `${error.message}`,
        });
        setUploaded(false);
        setValidated("fail");
        setFile(null);
      }
    } else {
      setError(true);
      setValidated("");
    }
  }

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
        <form ref={ref} action={generateData}>
          <input
            type="file"
            accept=".xlsx,.csv"
            className={css({
              m: "20px 0px",
              p: "10px",
              h: "50px",
              w: "50%",
              border: "2px solid",
            })}
            onChange={(e: React.FormEvent<HTMLInputElement>) => {
              if (e.currentTarget.files && e.currentTarget.files[0]) {
                setError(false);
                setFile(e.currentTarget.files[0]);
              }
            }}
          />
          {error && <ErrorText />}

          <Flex
            w={uploaded ? "58%" : validated === "success" ? "85%" : "75%"}
            justifyContent="space-between"
          >
            <Button
              type="submit"
              size="lg"
              color="#fff"
              backgroundColor="#0074d9"
              m="10px 0px"
              fontWeight="bold"
              fontSize="lg"
              h="55px"
              _hover={{
                backgroundColor: "#56a1d1",
                borderColor: "#56a1d1",
              }}
            >
              <UploadCloud />
              Upload File
            </Button>
            {uploaded && <ValidationText />}
            {validated !== "" &&
              (validated === "success" ? (
                spreadsheet && (
                  <a
                    href={URL.createObjectURL(spreadsheet)}
                    download={`${spreadsheet.name}`}
                  >
                    <ValidatedButton variant={validated} />
                  </a>
                )
              ) : (
                <ValidatedButton variant={validated} />
              ))}
          </Flex>
        </form>
      </Box>
    </Flex>
  );
}
