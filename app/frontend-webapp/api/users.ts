"use server";

import { request } from "@/lib/api";
import { User } from "@/types/users";

const fetchKeys = {
  search: "users/unaffiliated",
};

/**
 * Get users who are unaffiliated with a study, and email matches the query.
 * @param studyId id of study to filter by.
 * @param query query to match user emails by.
 * @returns a list of users that match the criteria.
 */
export async function getUnaffiliated(
  studyId: number,
  query: string
): Promise<User[]> {
  try {
    const payload = {
      studyId: studyId,
      query: query,
    };
    return await request(fetchKeys.search, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
      },
      body: JSON.stringify(payload),
    });
  } catch (error) {
    console.warn("Failed to fetch users");
    return [];
  }
}
