"use server";

import request from "./request";

const fetchKeys = {
  generate: "syntheticdata/generate",
};

export async function postSpreadsheet(prevState: any, formData: FormData) {
  try {
    const response: File = await request(fetchKeys.generate, {
      method: "POST",
      body: formData,
    });

    return { message: "success", response };
  } catch (error) {
    console.error(error);
    return { message: "Failed to upload file." };
  }
}
