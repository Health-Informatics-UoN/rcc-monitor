"use server";

import request from "./request";

const fetchKeys = {
  generate: "syntheticdata/generate",
};

export async function postSpreadsheet(prevState: any, formData: FormData) {
  try {
    const response: string = await request(fetchKeys.generate, {
      method: "POST",
      body: formData,
    });
    console.log("🌳");

    return { message: "success", url: response };
  } catch (error) {
    console.error(error);
    return { message: "Failed to upload file." };
  }
}
