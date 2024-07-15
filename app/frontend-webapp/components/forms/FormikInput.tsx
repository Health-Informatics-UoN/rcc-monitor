import { Field, FieldAttributes, useField } from "formik";
import React from "react";

import { FormHelpError } from "@/components/forms/FormHelpErrors";

import { Input } from "../ui/input";
import { Label } from "../ui/label";

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
      // className="grid grid-cols-auto sm:grid-cols-1 items-center gap-3 w-full max-w-sm"
      className="grid items-center gap-3 w-full max-w-sm"
    >
      {label && (
        <Label htmlFor={name} className={meta.error && "text-red-500"}>
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
