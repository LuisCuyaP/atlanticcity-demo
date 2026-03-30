"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Spinner } from "@/components/ui/Spinner";
import { Alert } from "@/components/ui/Alert";

export default function LogoutPage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;

    async function run() {
      try {
        const response = await fetch("/api/auth/logout", { method: "POST" });
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
          if (!cancelled) setError(`${response.status} - ${message}`);
          return;
        }

        router.replace("/login");
      } catch (e) {
        if (!cancelled) {
          setError(e instanceof Error ? e.message : "Unexpected error");
        }
      }
    }

    run();
    return () => {
      cancelled = true;
    };
  }, [router]);

  return (
    <div className="flex flex-1 items-center justify-center bg-zinc-50 px-6 py-12 dark:bg-black">
      <div className="w-full max-w-md rounded-lg border border-zinc-200 bg-white p-6 dark:border-zinc-800 dark:bg-black">
        <h1 className="text-xl font-semibold text-zinc-900 dark:text-zinc-100">
          Cerrando sesión
        </h1>
        <p className="mt-1 text-sm text-zinc-600 dark:text-zinc-400">
          Eliminando cookies…
        </p>

        <div className="mt-6">
          {error ? <Alert title="Error" message={error} /> : <Spinner label="Saliendo" />}
        </div>
      </div>
    </div>
  );
}
