import { url } from "@/constants";

export async function getReports() {
  const res = await fetch(`${url}/api/reports`);

  if (!res.ok) {
    const errorMessage = `Failed to fetch data. Status: ${res.status} ${res.statusText}`;
    console.error(errorMessage);
    throw new Error(errorMessage);
  }

  return res.json();
}
