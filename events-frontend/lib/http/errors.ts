export type ApiErrorPayload = {
  message: string;
  statusCode?: number;
  code?: string;
};

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null;
}

function readString(value: unknown): string | null {
  return typeof value === "string" && value.trim().length > 0 ? value : null;
}

export async function readErrorMessageFromResponse(
  response: Response,
): Promise<string> {
  const contentType = response.headers.get("content-type") ?? "";

  try {
    if (contentType.includes("application/json")) {
      const data: unknown = await response.json();

      if (isRecord(data)) {
        const message =
          readString(data.message) ||
          readString(data.error) ||
          readString(data.detail) ||
          readString(data.title);

        if (message) return message;

        const errorObj = data.Error;
        if (isRecord(errorObj)) {
          const desc = readString(errorObj.Description);
          if (desc) return desc;
        }
      }

      return typeof data === "string" ? data : JSON.stringify(data);
    }

    const text = await response.text();
    if (text.trim().length > 0) return text;
  } catch {
    // ignore
  }

  return response.statusText || "Request failed";
}
