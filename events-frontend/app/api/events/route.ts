import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { AUTH_COOKIE_NAME } from "@/lib/auth/constants";
import { getApiBaseUrl } from "@/lib/backend/config";
import { readErrorMessageFromResponse } from "@/lib/http/errors";

export async function GET(): Promise<Response> {
  try {
    const cookieStore = await cookies();
    const token = cookieStore.get(AUTH_COOKIE_NAME)?.value;
    if (!token) throw new Error("Unauthorized: missing JWT cookie");
    const url = `${getApiBaseUrl()}/events`;

    const response = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    });

    if (!response.ok) {
      const message = await readErrorMessageFromResponse(response);
      return NextResponse.json(
        { message, statusCode: response.status },
        { status: response.status },
      );
    }

    const data = await response.json();
    return NextResponse.json(data, { status: 200 });
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unexpected error";
    const statusCode = message.toLowerCase().includes("unauthorized") ? 401 : 500;

    return NextResponse.json(
      { message, statusCode },
      { status: statusCode },
    );
  }
}

export async function POST(request: Request): Promise<Response> {
  try {
    const cookieStore = await cookies();
    const token = cookieStore.get(AUTH_COOKIE_NAME)?.value;
    if (!token) throw new Error("Unauthorized: missing JWT cookie");
    const url = `${getApiBaseUrl()}/events`;

    const body = await request.text();

    const response = await fetch(url, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": request.headers.get("content-type") || "application/json",
      },
      body,
    });

    if (!response.ok) {
      const message = await readErrorMessageFromResponse(response);
      return NextResponse.json(
        { message, statusCode: response.status },
        { status: response.status },
      );
    }

    const contentType = response.headers.get("content-type") ?? "";
    if (contentType.includes("application/json")) {
      const data = await response.json();
      return NextResponse.json(data, { status: response.status });
    }

    const text = await response.text();
    return NextResponse.json(
      { message: text || "OK" },
      { status: response.status },
    );
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unexpected error";
    const statusCode = message.toLowerCase().includes("unauthorized") ? 401 : 500;

    return NextResponse.json(
      { message, statusCode },
      { status: statusCode },
    );
  }
}
