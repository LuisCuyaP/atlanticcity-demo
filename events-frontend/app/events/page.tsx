"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { Navbar } from "@/components/Navbar";
import { Spinner } from "@/components/ui/Spinner";
import { Alert } from "@/components/ui/Alert";
import type { EventListItem } from "@/lib/events/types";
import { apiFetchJson } from "@/lib/http/client";

export default function EventsPage() {
  const router = useRouter();
  const [items, setItems] = useState<EventListItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setIsLoading(true);
      setError(null);

      const result = await apiFetchJson<EventListItem[]>("/api/events", {
        method: "GET",
      });

      if (cancelled) return;

      if (!result.ok) {
        if (result.error.statusCode === 401) {
          router.replace("/login");
          return;
        }
        const msg = result.error.statusCode
          ? `${result.error.statusCode} - ${result.error.message}`
          : result.error.message;
        setError(msg);
        setItems([]);
      } else {
        setItems(result.data);
      }

      setIsLoading(false);
    }

    load();
    return () => {
      cancelled = true;
    };
  }, [router]);

  return (
    <div className="min-h-full bg-zinc-50 dark:bg-black">
      <Navbar />
      <main className="mx-auto w-full max-w-5xl px-6 py-10">
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-semibold text-zinc-900 dark:text-zinc-100">
            Eventos
          </h1>
          <Link
            href="/events/new"
            className="rounded-md bg-zinc-900 px-4 py-2 text-sm font-medium text-white hover:bg-zinc-800 dark:bg-zinc-100 dark:text-zinc-900 dark:hover:bg-zinc-200"
          >
            Registrar evento
          </Link>
        </div>

        <div className="mt-6">
          {isLoading ? (
            <Spinner label="Cargando eventos" />
          ) : error ? (
            <Alert title="Error" message={error} />
          ) : items.length === 0 ? (
            <p className="text-sm text-zinc-600 dark:text-zinc-400">
              No hay eventos.
            </p>
          ) : (
            <div className="grid grid-cols-1 gap-3">
              {items.map((e) => (
                <div
                  key={e.id}
                  className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-black"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <div className="text-sm font-semibold text-zinc-900 dark:text-zinc-100">
                        {e.name}
                      </div>
                      <div className="mt-1 text-sm text-zinc-600 dark:text-zinc-400">
                        {new Date(e.eventDate).toLocaleString()} · {e.place}
                      </div>
                    </div>
                    <div className="text-xs rounded-full border border-zinc-200 px-2 py-1 text-zinc-700 dark:border-zinc-800 dark:text-zinc-300">
                      {e.status}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
