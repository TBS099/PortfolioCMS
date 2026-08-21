import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";

interface LoadingStateProps {
  className?: string;
}

function LoadingState({ className }: LoadingStateProps) {
  return (
    <div
      className={cn("flex items-center justify-center h-48", className)}
    >
      <div className="w-full max-w-sm space-y-3">
        <Skeleton className="h-4 w-3/4 mx-auto" />
        <Skeleton className="h-4 w-1/2 mx-auto" />
      </div>
    </div>
  );
}

export { LoadingState };
