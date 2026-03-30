"use client";

import Link from "next/link";
import { LogoutButton } from "@/components/LogoutButton";

export function Navbar() {
  return (
    <header className="border-b border-zinc-200 bg-white dark:border-zinc-800 dark:bg-black">
      <div className="mx-auto flex w-full max-w-5xl items-center justify-between px-6 py-4">
        <div className="flex items-center gap-6">
          <Link
            href="/events"
            className="text-sm font-semibold text-zinc-900 dark:text-zinc-100"
          >
            Events
          </Link>
          <nav className="flex items-center gap-4">
            <Link
              href="/events"
              className="text-sm text-zinc-700 hover:text-zinc-900 dark:text-zinc-300 dark:hover:text-zinc-100"
            >
              Listar
            </Link>
            <Link
              href="/events/new"
              className="text-sm text-zinc-700 hover:text-zinc-900 dark:text-zinc-300 dark:hover:text-zinc-100"
            >
              Registrar
            </Link>
          </nav>
        </div>
        <LogoutButton />
      </div>
    </header>
  );
}
