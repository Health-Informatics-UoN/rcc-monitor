"use server";

import request from "./request";

const fetchKeys = {
  generate: "syntheticdata/generate",
  getFile: (filename: string) => `syntheticdata/file?name=${filename}`,
};

/**
 * Upload the spreadsheet to the backend.
 * @param prevState State of the form.
 * @param formData Formdata to upload.
 * @returns
 */
export async function postSpreadsheet(prevState: any, formData: FormData) {
  try {
    const response: { name: string } = await request(fetchKeys.generate, {
      method: "POST",
      body: formData,
    });

    return { message: "success", name: response.name };
  } catch (error) {
    console.error(error);
    return { message: "Failed to upload file." };
  }
}

/**
 * Get file from the backend.
 * @param filename name of the file to get
 * @returns File Blob
 */
export async function getFile(filename: string): Promise<Blob | undefined> {
  try {
    const response = await request<Blob>(fetchKeys.getFile(filename), {
      method: "GET",
      download: true,
    });

    return response;
  } catch (error) {
    console.error(error);
  }
}
