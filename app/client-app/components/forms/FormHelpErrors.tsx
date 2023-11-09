import { css } from "@/styled-system/css";

import { Label } from "@/components/ui/label";

interface FormHelpErrorProps {
  isInvalid: unknown;
  help?: string;
  collapseEmpty?: boolean;
  replaceHelpWithError?: boolean;
  error?: string;
}

export const FormHelpError = ({
  isInvalid,
  help,
  collapseEmpty,
  replaceHelpWithError,
  error,
}: FormHelpErrorProps) => {
  // collapseEmpty defaults to false so we
  // always display text containers (even if empty)
  // so the layout has a placeholder in case of errors,
  // preventing vertical shift when text appears.

  // help component, if any
  const displayHelp =
    collapseEmpty && !help ? null : <Label>{help || <>&nbsp;</>}</Label>;

  // error component, if any
  const displayError =
    collapseEmpty && !error ? null : (
      <Label className={css({ color: "destructive" })}>
        {error || <>&nbsp;</>}
      </Label>
    );

  // these are our actual rendered slots
  const helpSlot = replaceHelpWithError && isInvalid ? null : displayHelp;
  const errorSlot = replaceHelpWithError && !isInvalid ? null : displayError;

  return (
    <>
      {helpSlot}
      {errorSlot}
    </>
  );
};
