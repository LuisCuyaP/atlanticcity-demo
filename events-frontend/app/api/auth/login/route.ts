import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { AUTH_COOKIE_NAME } from "@/lib/auth/constants";
import { getApiBaseUrl } from "@/lib/backend/config";
import { readErrorMessageFromResponse } from "@/lib/http/errors";

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null;
}

type LoginRequest = {
  userName: string;
  password: string;
};

export async function POST(request: Request): Promise<Response> {
  let body: Partial<LoginRequest>;
  try {
    body = (await request.json()) as Partial<LoginRequest>;
  } catch {
    return NextResponse.json(
      { message: "Invalid request body", statusCode: 400 },
      { status: 400 },
    );
  }

  try {

    const userName = (body.userName ?? "").trim();
    const password = (body.password ?? "").trim();

    if (!userName || !password) {
      return NextResponse.json(
        {
          message: "Missing required fields: userName, password",
          statusCode: 400,
        },
        { status: 400 },
      );
    }

    const response = await fetch(`${getApiBaseUrl()}/auth/login`, {
      method: "POST",
      headers: {
        Accept: "*/*",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        userName,
        password,
      }),
      cache: "no-store",
    });

    if (!response.ok) {
      const message = await readErrorMessageFromResponse(response);
      return NextResponse.json(
        { message, statusCode: response.status },
        { status: response.status },
      );
    }

    const data: unknown = await response.json();
    const accessToken =
      isRecord(data) && typeof data.accessToken === "string"
        ? data.accessToken.trim()
        : "";

    if (!accessToken) {
      return NextResponse.json(
        {
          message: "Login response did not include accessToken",
          statusCode: 502,
        },
        { status: 502 },
      );
    }

    const cookieStore = await cookies();
    cookieStore.set({
      name: AUTH_COOKIE_NAME,
      value: accessToken,
      httpOnly: true,
      sameSite: "lax",
      secure: process.env.NODE_ENV === "production",
      path: "/",
      maxAge: 60 * 60 * 8, // 8h
    });

    return NextResponse.json({ ok: true }, { status: 200 });
  } catch (error) {
    const message = error instanceof Error ? error.message : "Unexpected error";
    return NextResponse.json(
      { message, statusCode: 500 },
      { status: 500 },
    );
  }
}
