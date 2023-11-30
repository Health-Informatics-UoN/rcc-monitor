// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
import { useFormStatus } from "react-dom";

import { type HTMLStyledProps } from "@/styled-system/jsx";
import { Button } from "@/components/shadow-ui/Button";

export function Submit({ children }: HTMLStyledProps<typeof Button>) {
  const { pending } = useFormStatus();

  return (
    <Button type="submit" disabled={pending}>
      {children}
    </Button>
  );
}
