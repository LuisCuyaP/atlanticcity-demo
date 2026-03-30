type AlertProps = {
  title?: string;
  message: string;
  variant?: "error" | "info";
};

const variants: Record<NonNullable<AlertProps["variant"]>, string> = {
  error:
    "border-red-200 bg-red-50 text-red-800 dark:border-red-900/40 dark:bg-red-950/30 dark:text-red-200",
  info:
    "border-zinc-200 bg-zinc-50 text-zinc-800 dark:border-zinc-800 dark:bg-zinc-950/30 dark:text-zinc-200",
};

export function Alert({ title, message, variant = "error" }: AlertProps) {
  return (
    <div className={"rounded-md border p-3 text-sm " + variants[variant]}>
      {title ? <div className="mb-1 font-medium">{title}</div> : null}
      <div className="whitespace-pre-wrap">{message}</div>
    </div>
  );
}
