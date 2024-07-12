import { Button } from "../ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "../ui/dialog";

interface ConfirmationDialogProps {
  css?: string;
  refProp: React.RefObject<HTMLButtonElement>;
  title: string;
  description: string;
  leftButtonName: string;
  rightButtonName: string;
  handleClick: React.MouseEventHandler<HTMLButtonElement>;
}

export function ConfirmationDialog({
  css,
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
          <DialogDescription className="mt-5">{description}</DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogClose className="mt-[50px]" asChild>
            <div className=" flex justify-end gap-3">
              <Button variant="secondary">{leftButtonName}</Button>
              <Button className={css} onClick={handleClick}>
                {rightButtonName}
              </Button>
            </div>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
