"use server";

import { revalidatePath } from "next/cache";

import { ApiError, request } from "@/lib/api";
import { Study, StudyPartial } from "@/types/studies";

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
      studyGroup: [],
      studyCapacityAlert: false,
      productionSubjectsEnteredAlert: false,
      instance: "Build",
      studyCapacityAlertsActivated: false,
      studyCapacityThreshold: 0,
      studyCapacityJobFrequency: "",
      studyCapacityLastChecked: "",
      subjectsEnrolled: 0,
      subjectsEnrolledThreshold: 0,
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
export async function validateStudy(formData: {
  apiKey: string;
}): Promise<Study> {
  return await request<Study>(fetchKeys.validate, {
    method: "POST",
    headers: {
      "Content-type": "application/json",
    },
    body: JSON.stringify(formData),
  });
}

/**
 * Form action to add a study.
 * @param prevState previous form state
 * @param formData form data to POST.
 */
export async function addStudy(formData: Study) {
  const response = await request<Study>(fetchKeys.create, {
    method: "POST",
    headers: {
      "Content-type": "application/json",
    },
    body: JSON.stringify(formData),
  });

  revalidatePath("/studies");
}
