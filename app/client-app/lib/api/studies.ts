"use server";

import { Study, StudyPartial } from "@/types/studies";
import request from "./request";
import { ApiError } from "./error";
import { revalidatePath } from "next/cache";

const fetchKeys = {
  list: "studies",
  get: (id: number) => `studies/${id}`,
  update: (id: number) => `studies/${id}`,
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
 * Get a study with a given id.
 * @param id Study to get
 * @returns The study matching the given id
 */
export async function getStudy(id: number): Promise<StudyPartial> {
  try {
    return await request<StudyPartial>(fetchKeys.get(id));
  } catch (error) {
    console.warn("Failed to fetch study.");
    return {
      id: 0,
      name: "",
      users: [],
    };
  }
}

/**
 * Form action to update a study.
 * @param prevState previous form state
 * @param formData form data to update.
 */
export async function updateStudy(model: StudyPartial) {
  return request<StudyPartial>(fetchKeys.update(model.id), {
    method: "PUT",
    headers: {
      "Content-type": "application/json",
    },
    body: JSON.stringify(model),
  });
}

/**
 * Form action to delete a study.
 * @param id id of study to DELETE.
 */
export async function deleteStudy(id: number) {
  try {
    await request(fetchKeys.delete(id), {
      method: "DELETE",
    });
    revalidatePath("/studies");

    return { success: true };
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
    const payload = JSON.stringify(Object.fromEntries(formData.entries()));

    const response = await request<Study>(fetchKeys.validate, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
      },
      body: payload,
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
    const payload = JSON.stringify(Object.fromEntries(formData.entries()));

    const response = await request<Study>(fetchKeys.create, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
      },
      body: payload,
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
