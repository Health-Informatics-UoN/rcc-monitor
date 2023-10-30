import { experimental_useFormStatus as useFormStatus } from "react-dom";
import { type HTMLStyledProps } from "@/styled-system/jsx";
import { Button } from "@/components/ui/button";

export function Submit({ children }: HTMLStyledProps<typeof Button>) {
  const { pending } = useFormStatus();

  return (
    <Button type="submit" disabled={pending}>
      {children}
    </Button>
  );
}
