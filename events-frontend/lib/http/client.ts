import type { ApiErrorPayload } from "@/lib/http/errors";

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null;
}

export async function apiFetchJson<T>(
  input: RequestInfo | URL,
  init?: RequestInit,
): Promise<{ ok: true; data: T } | { ok: false; error: ApiErrorPayload }> {
  try {
    const headers = new Headers(init?.headers);
    if (init?.body && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    const response = await fetch(input, {
      ...init,
      headers,
    });

    if (!response.ok) {
      let message = response.statusText || "Request failed";
      try {
        const data: unknown = await response.json();
        if (typeof data === "string") {
          message = data;
        } else if (isRecord(data)) {
          const msg = data.message;
          const detail = data.detail;
          const title = data.title;
          if (typeof msg === "string" && msg.trim()) message = msg;
          else if (typeof detail === "string" && detail.trim()) message = detail;
          else if (typeof title === "string" && title.trim()) message = title;
        }
      } catch {
        // ignore
      }

      return {
        ok: false,
        error: { message, statusCode: response.status },
      };
    }

    const data = (await response.json()) as T;
    return { ok: true, data };
  } catch (error) {
    const message = error instanceof Error ? error.message : "Network error";
    return { ok: false, error: { message } };
  }
}
