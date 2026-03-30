type SpinnerProps = {
  className?: string;
  label?: string;
};

export function Spinner({ className, label }: SpinnerProps) {
  return (
    <div className={"inline-flex items-center gap-2"} aria-live="polite">
      <span
        className={
          "inline-block h-4 w-4 animate-spin rounded-full border-2 border-zinc-300 border-t-zinc-900 dark:border-zinc-700 dark:border-t-zinc-100 " +
          (className ?? "")
        }
        aria-hidden="true"
      />
      {label ? (
        <span className="text-sm text-zinc-700 dark:text-zinc-200">{label}</span>
      ) : null}
    </div>
  );
}
