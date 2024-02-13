import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/shadow-ui/Dialog";
import { Flex } from "@/styled-system/jsx";
import { Button } from "@/components/shadow-ui/Button";
import { css } from "@/styled-system/css";

interface ConfirmationDialogProps {
  css?: object;
  refProp: React.RefObject<HTMLButtonElement>;
  title: string;
  description: string;
  leftButtonName: string;
  rightButtonName: string;
  handleClick: React.MouseEventHandler<HTMLButtonElement>;
}

export function ConfirmationDialog({
  css: cssProp = {},
  refProp,
  title,
  description,
  leftButtonName,
  rightButtonName,
  handleClick,
}: ConfirmationDialogProps) {
  return (
    <Dialog>
      <DialogTrigger ref={refProp} />
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription mt={5}>{description}</DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogClose className={css({ mt: "50px" })} asChild>
            <Flex justifyContent="end" gap={3}>
              <Button variant="secondary">{leftButtonName}</Button>
              <Button className={css(cssProp)} onClick={handleClick}>
                {rightButtonName}
              </Button>
            </Flex>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
