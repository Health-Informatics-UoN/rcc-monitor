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
export async function postSpreadsheet(
  formData: FormData
): Promise<{ name: string }> {
  const response: { name: string } = await request(fetchKeys.generate, {
    method: "POST",
    body: formData,
  });

  return { name: response.name };
}

/**
 * Get file from the backend.
 * @param filename name of the file to get
 * @returns File Blob
 */
export async function getFile(filename: string): Promise<Blob | undefined> {
  try {
    return await request<Blob>(fetchKeys.getFile(filename), {
      method: "GET",
      download: true,
    });
  } catch (error) {
    console.error(error);
  }
}
