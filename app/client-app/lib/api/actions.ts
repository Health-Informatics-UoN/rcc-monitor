"use server";
import request from "./request";

export async function postSpreadsheet(formData: FormData): Promise<File> {
  try {
    const response: File = await request("synthetic-data", {
      method: "POST",
      body: formData,
    });

    return response;
  } catch (error) {
    console.error(error);
    return Promise.reject(error);
  }
}
