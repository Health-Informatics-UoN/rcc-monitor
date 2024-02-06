import { getServerSession } from "next-auth";

import { apiUrl as apiUrl } from "@/constants";
import { options as authOptions } from "@/lib/auth";

import { ApiError } from "./error";

interface RequestOptions {
  method?: string;
  headers?: HeadersInit;
  body?: BodyInit;
  download?: boolean;
  cache?: RequestCache;
  next?: { revalidate: number };
}

/**
 * Base request function to query the backend API with a users access token
 * @param url URL to query.
 * @param options RequestOptions object
 * @returns The given type T
 */
const request = async <T>(
  url: string,
  options: RequestOptions = {}
): Promise<T> => {
  // Get the KeyCloak id_token
  const session = await getServerSession(authOptions);
  const token = session?.access_token;

  const headers: HeadersInit = {
    Authorization: `Bearer ${token}`,
    ...(options.headers || {}),
  };

  const response = await fetch(`${apiUrl}/api/${url}`, {
    method: options.method || "GET",
    headers: headers,
    body: options.body,
    cache: options.cache,
    next: options.next,
  });

  if (!response.ok) {
    const errorMessage = await response.text();
    throw new ApiError(errorMessage, response.status);
  }

  if (options.download) {
    return response.blob() as unknown as T;
  }

  if (response.status === 204) {
    return {} as T;
  }

  return response.json();
};

export default request;
