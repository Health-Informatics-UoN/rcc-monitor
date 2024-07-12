import { ReactNode } from "react";
import { useFormStatus } from "react-dom";

import { Button } from "../ui/button";

export function Submit({ children }: { children: ReactNode | ReactNode[] }) {
  const { pending } = useFormStatus();

  return (
    <Button type="submit" disabled={pending}>
      {children}
    </Button>
  );
}
