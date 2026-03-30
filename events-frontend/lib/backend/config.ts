export function getApiBaseUrl(): string {
  const baseUrl = process.env.API_BASE_URL ?? process.env.EVENTS_API_BASE_URL;
  if (baseUrl) return baseUrl.replace(/\/$/, "");

  // Developer-friendly default (matches your local backend example)
  if (process.env.NODE_ENV !== "production") {
    return "http://localhost:5143";
  }

  throw new Error("Missing env var API_BASE_URL (e.g. http://localhost:5143)");
}
