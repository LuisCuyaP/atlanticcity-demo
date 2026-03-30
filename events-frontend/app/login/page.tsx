"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Alert } from "@/components/ui/Alert";

export default function LoginPage() {
  const router = useRouter();
  const [userName, setUserName] = useState("admin");
  const [password, setPassword] = useState("admin");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (
      !userName.trim() ||
      !password.trim()
    ) {
      setError("Todos los campos son obligatorios");
      return;
    }

    setIsLoading(true);
    try {
      const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          userName: userName.trim(),
          password: password.trim(),
        }),
      });

      if (!response.ok) {
        let message = response.statusText;
        try {
          const data: unknown = await response.json();
          if (typeof data === "string") message = data;
          else if (typeof data === "object" && data !== null) {
            const record = data as Record<string, unknown>;
            const msg = record.message;
            const detail = record.detail;
            const title = record.title;
            if (typeof msg === "string" && msg.trim()) message = msg;
            else if (typeof detail === "string" && detail.trim()) message = detail;
            else if (typeof title === "string" && title.trim()) message = title;
          }
        } catch {
          // ignore
        }
        setError(`${response.status} - ${message}`);
        return;
      }

      router.replace("/events");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="flex flex-1 items-center justify-center bg-zinc-50 px-6 py-12 dark:bg-black">
      <div className="w-full max-w-md rounded-lg border border-zinc-200 bg-white p-6 dark:border-zinc-800 dark:bg-black">
        <h1 className="text-xl font-semibold text-zinc-900 dark:text-zinc-100">
          Iniciar sesión
        </h1>
        <p className="mt-1 text-sm text-zinc-600 dark:text-zinc-400">
          Autenticación demo
        </p>

        <form onSubmit={onSubmit} className="mt-6 flex flex-col gap-4">
          <Input
            label="Usuario"
            name="userName"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            autoComplete="username"
          />

          <Input
            label="Password"
            name="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
          />

          {error ? <Alert title="Error" message={error} /> : null}

          <Button type="submit" isLoading={isLoading}>
            Entrar
          </Button>
        </form>
      </div>
    </div>
  );
}
