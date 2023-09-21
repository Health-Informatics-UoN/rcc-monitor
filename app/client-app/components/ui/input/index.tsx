import { styled, type HTMLStyledProps } from "@/styled-system/jsx";
import { input } from "@/styled-system/recipes";

export const Input = styled("input", input);
export type InputProps = HTMLStyledProps<typeof Input>;
