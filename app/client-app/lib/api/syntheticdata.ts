"use server";

import request from "./request";

const fetchKeys = {
  generate: "syntheticdata/generate",
};

export async function postSpreadsheet(
  prevState: any,
  formData: FormData
): Promise<File> {
  try {
    // Transform file.
    const newFormData = new FormData();
    const file: File | null = formData.get("file") as unknown as File;
    const bytes = await file.arrayBuffer();
    const blob = new Blob([bytes]);
    newFormData.append("file", blob);

    const response: File = await request(fetchKeys.generate, {
      method: "POST",
      body: newFormData,
    });

    return response;
  } catch (error) {
    console.error(error);
    return Promise.reject(error);
  }
}
