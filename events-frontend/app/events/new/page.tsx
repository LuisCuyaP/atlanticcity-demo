"use client";

import { useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { Navbar } from "@/components/Navbar";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Alert } from "@/components/ui/Alert";
import type { CreateEventRequestDto } from "@/lib/events/types";
import { apiFetchJson } from "@/lib/http/client";

type ZoneRow = {
  id: string;
  name: string;
  price: string;
  capacity: string;
};

function newId() {
  return typeof crypto !== "undefined" && "randomUUID" in crypto
    ? crypto.randomUUID()
    : Math.random().toString(16).slice(2);
}

export default function NewEventPage() {
  const router = useRouter();

  const [name, setName] = useState("");
  const [eventDate, setEventDate] = useState("");
  const [place, setPlace] = useState("");
  const [zones, setZones] = useState<ZoneRow[]>([
    { id: newId(), name: "General", price: "0", capacity: "1" },
  ]);

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const fieldErrors = useMemo(() => {
    const errors: Record<string, string> = {};

    if (!name.trim()) errors.name = "Nombre es obligatorio";
    if (!eventDate.trim()) errors.eventDate = "Fecha es obligatoria";
    if (!place.trim()) errors.place = "Lugar es obligatorio";

    zones.forEach((z, index) => {
      if (!z.name.trim()) errors[`zone_${index}_name`] = "Nombre de zona obligatorio";

      const price = Number(z.price);
      if (Number.isNaN(price) || price < 0) {
        errors[`zone_${index}_price`] = "Precio debe ser >= 0";
      }

      const capacity = Number(z.capacity);
      if (Number.isNaN(capacity) || capacity <= 0) {
        errors[`zone_${index}_capacity`] = "Capacidad debe ser > 0";
      }
    });

    return errors;
  }, [name, eventDate, place, zones]);

  function addZone() {
    setZones((prev) => [
      ...prev,
      { id: newId(), name: "", price: "0", capacity: "1" },
    ]);
  }

  function removeZone(id: string) {
    setZones((prev) => prev.filter((z) => z.id !== id));
  }

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (Object.keys(fieldErrors).length > 0) {
      setError("Hay campos inválidos. Revisa el formulario.");
      return;
    }

    setIsLoading(true);
    try {
      const payload: CreateEventRequestDto = {
        Name: name.trim(),
        // datetime-local already returns ISO-like local time without timezone.
        // This avoids accidental UTC offset shifts.
        EventDate: eventDate,
        Place: place.trim(),
        Zones: zones.map((z) => ({
          Name: z.name.trim(),
          Price: Number(z.price),
          Capacity: Number(z.capacity),
        })),
      };

      const result = await apiFetchJson<unknown>("/api/events", {
        method: "POST",
        body: JSON.stringify(payload),
      });

      if (!result.ok) {
        if (result.error.statusCode === 401) {
          router.replace("/login");
          return;
        }
        const msg = result.error.statusCode
          ? `${result.error.statusCode} - ${result.error.message}`
          : result.error.message;
        setError(msg);
        return;
      }

      const data = result.data;
      const eventId =
        typeof data === "string"
          ? data
          : typeof data === "object" && data !== null
            ? (() => {
                const record = data as Record<string, unknown>;
                const id = record.id;
                const value = record.value;
                if (typeof id === "string" && id.trim()) return id;
                if (typeof value === "string" && value.trim()) return value;
                return null;
              })()
            : null;

      setSuccess(
        `Evento creado${eventId ? `: ${eventId}` : ""}. Redirigiendo a la lista…`,
      );

      setTimeout(() => {
        router.replace("/events");
      }, 900);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="min-h-full bg-zinc-50 dark:bg-black">
      <Navbar />
      <main className="mx-auto w-full max-w-5xl px-6 py-10">
        <h1 className="text-2xl font-semibold text-zinc-900 dark:text-zinc-100">
          Registrar evento
        </h1>
        <p className="mt-1 text-sm text-zinc-600 dark:text-zinc-400">
          Crea un evento con zonas (nombre, precio, capacidad).
        </p>

        <form onSubmit={onSubmit} className="mt-6 flex flex-col gap-6">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
            <Input
              label="Nombre"
              name="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              error={fieldErrors.name}
            />

            <Input
              label="Fecha"
              name="eventDate"
              type="datetime-local"
              value={eventDate}
              onChange={(e) => setEventDate(e.target.value)}
              error={fieldErrors.eventDate}
            />

            <Input
              label="Lugar"
              name="place"
              value={place}
              onChange={(e) => setPlace(e.target.value)}
              error={fieldErrors.place}
            />
          </div>

          <section className="rounded-lg border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-black">
            <div className="flex items-center justify-between">
              <h2 className="text-sm font-semibold text-zinc-900 dark:text-zinc-100">
                Zonas
              </h2>
              <Button type="button" variant="secondary" onClick={addZone}>
                Agregar zona
              </Button>
            </div>

            <div className="mt-4 flex flex-col gap-3">
              {zones.map((z, index) => (
                <div
                  key={z.id}
                  className="grid grid-cols-1 gap-3 md:grid-cols-12"
                >
                  <div className="md:col-span-5">
                    <Input
                      label="Nombre"
                      name={`zone_${index}_name`}
                      value={z.name}
                      onChange={(e) =>
                        setZones((prev) =>
                          prev.map((x) =>
                            x.id === z.id ? { ...x, name: e.target.value } : x,
                          ),
                        )
                      }
                      error={fieldErrors[`zone_${index}_name`]}
                    />
                  </div>

                  <div className="md:col-span-3">
                    <Input
                      label="Precio"
                      name={`zone_${index}_price`}
                      type="number"
                      inputMode="decimal"
                      value={z.price}
                      onChange={(e) =>
                        setZones((prev) =>
                          prev.map((x) =>
                            x.id === z.id ? { ...x, price: e.target.value } : x,
                          ),
                        )
                      }
                      error={fieldErrors[`zone_${index}_price`]}
                      min={0}
                      step="0.01"
                    />
                  </div>

                  <div className="md:col-span-3">
                    <Input
                      label="Capacidad"
                      name={`zone_${index}_capacity`}
                      type="number"
                      inputMode="numeric"
                      value={z.capacity}
                      onChange={(e) =>
                        setZones((prev) =>
                          prev.map((x) =>
                            x.id === z.id
                              ? { ...x, capacity: e.target.value }
                              : x,
                          ),
                        )
                      }
                      error={fieldErrors[`zone_${index}_capacity`]}
                      min={1}
                      step={1}
                    />
                  </div>

                  <div className="md:col-span-1 md:flex md:items-end">
                    <Button
                      type="button"
                      variant="danger"
                      onClick={() => removeZone(z.id)}
                      disabled={zones.length === 1}
                      className="w-full"
                    >
                      X
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </section>

          {error ? <Alert title="Error" message={error} /> : null}
          {success ? (
            <Alert title="Éxito" message={success} variant="info" />
          ) : null}

          <div className="flex items-center gap-3">
            <Button type="submit" isLoading={isLoading}>
              Guardar
            </Button>
            <Button
              type="button"
              variant="secondary"
              onClick={() => router.push("/events")}
              disabled={isLoading}
            >
              Cancelar
            </Button>
          </div>
        </form>
      </main>
    </div>
  );
}
