"use server";

import { Study, StudyPartial } from "@/types/studies";
import request from "./request";
import { ApiError } from "./error";
import { revalidatePath } from "next/cache";

const fetchKeys = {
  list: "studies",
  delete: (id: number) => `studies/${id}`,
  validate: "studies/validate",
  create: "studies",
};

export async function getStudies(): Promise<StudyPartial[]> {
  try {
    return await request<StudyPartial[]>(fetchKeys.list);
  } catch (error) {
    console.warn("Failed to fetch data.");
    return [];
  }
}

/**
 * Form action to delete a study.
 * @param id id of study to DELETE.
 */
export async function deleteStudy(id: number) {
  try {
    const response: { name: string } = await request(fetchKeys.delete(id), {
      method: "DELETE",
    });
    revalidatePath("/studies");

    return { success: true, name: response.name };
  } catch (error) {
    console.error(error);
    let message: string;
    if (error instanceof ApiError) message = error.message;
    else message = String(error);
    return { success: false, message: message };
  }
}

/**
 * Form action to validate a study.
 * @param prevState previous form state.
 * @param formData form data to POST.
 */
export async function validateStudy(prevState: unknown, formData: FormData) {
  try {
    const response = await request<Study>(fetchKeys.validate, {
      method: "POST",
      body: formData,
    });

    return { success: true, study: response };
  } catch (error) {
    console.warn(error);
    let message;
    if (error instanceof ApiError) message = error.message;
    else message = String(error);
    return { success: false, message: message };
  }
}

/**
 * Form action to add a study.
 * @param prevState previous form state
 * @param formData form data to POST.
 */
export async function addStudy(prevState: unknown, formData: FormData) {
  try {
    const response = await request<Study>(fetchKeys.create, {
      method: "POST",
      body: formData,
    });

    revalidatePath("/studies");

    return { success: true, study: response };
  } catch (error) {
    console.warn(error);
    let message;
    if (error instanceof ApiError) message = error.message;
    else message = String(error);
    return { success: false, message: message };
  }
}
