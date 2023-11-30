import { styled, type HTMLStyledProps } from "@/styled-system/jsx";
import { label } from "@/styled-system/recipes";

export const Label = styled("label", label);
export type LabelProps = HTMLStyledProps<typeof Label>;
