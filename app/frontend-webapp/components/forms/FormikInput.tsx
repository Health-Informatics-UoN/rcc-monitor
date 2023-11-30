import { useField, Field, FieldAttributes } from "formik";
import React from "react";
import { css } from "@/styled-system/css";

import { Input } from "@/components/shadow-ui/Input";
import { Label } from "@/components/shadow-ui/Label";
import { FormHelpError } from "@/components/forms/FormHelpErrors";

export interface FormikInputProps {
  name: string;
  label?: string;
  placeholder?: string;
  type?: string;
  fieldHelp?: string;
  accept?: string;
  collapseError?: boolean;
  onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

export const FormikInput: React.FC<
  FormikInputProps & FieldAttributes<object>
> = ({
  name,
  label,
  placeholder,
  type = "text",
  fieldHelp,
  accept,
  required,
  disabled,
  collapseError,
  onChange,
}) => {
  const [field, meta] = useField({ name, type });

  return (
    <div
      className={css({
        display: "grid",
        w: "full",
        maxW: "sm",
        alignItems: "center",
        gap: "3",
      })}
    >
      {label && (
        <Label
          htmlFor={name}
          className={meta.error && css({ color: "destructive" })}
        >
          {label}
        </Label>
      )}
      <Field
        as={Input}
        name={name}
        id={name}
        type={type}
        accept={accept}
        placeholder={placeholder}
        onChange={onChange || field.onChange}
        required={required}
        disabled={disabled}
      />

      <FormHelpError
        isInvalid={meta.error && meta.touched}
        error={meta.error}
        help={fieldHelp}
        collapseEmpty={collapseError}
        replaceHelpWithError
      />
    </div>
  );
};
