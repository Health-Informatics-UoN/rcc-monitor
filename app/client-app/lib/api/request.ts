import { url as apiUrl } from "@/constants";
import { getServerSession } from "next-auth";
import { options as authOptions } from "@/auth/options";

// Error class for API errors
class APIError extends Error {
  constructor(message: string, status: number) {
    super(message);
    this.name = "APIError";
  }
}

interface RequestOptions {
  method?: string;
  headers?: HeadersInit;
  body?: BodyInit;
  cache?: RequestCache;
  next?: { revalidate: number };
}

const defaultHeaders: HeadersInit = {
  "Content-Type": "application/json",
};

const request = async <T>(
  url: string,
  options: RequestOptions = {}
): Promise<T> => {
  try {
    // Get the KeyCloak id_token
    const session = await getServerSession(authOptions);
    const token = session?.id_token;

    const response = await fetch(`${apiUrl}/api/${url}`, {
      method: options.method || "GET",
      headers: {
        ...defaultHeaders,
        ...options.headers,
        Authorization: `Bearer ${token}`,
      },
      body: options.body,
      cache: options.cache,
      next: options.next,
    });

    if (!response.ok) {
      const errorMessage = await response.text();
      throw new APIError(errorMessage, response.status);
    }

    return response.json();
  } catch (error) {
    console.error(error);
    throw new APIError("Failed to fetch data", 500);
  }
};

export default request;
