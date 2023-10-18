"use server";

import request from "./request";

const fetchKeys = {
  generate: "syntheticdata/generate",
  get: (filename: string) => `syntheticdata/file?name=${filename}`,
};

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

export async function getSpreadsheet(filename: string) {
  try {
    const response = await request(fetchKeys.get(filename), {
      method: "GET",
      download: true,
    });

    return response;
  } catch (error) {
    console.error(error);
    return { message: "Failed to get file." };
  }
}
