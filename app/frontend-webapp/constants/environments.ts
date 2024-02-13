import { FacetsFilter } from "@/components/shared/Icons";

export const environments: FacetsFilter[] = [
  {
    value: "Build",
    label: "Build",
    icon: "Construction",
    color: "red.400",
  },
  {
    value: "UAT",
    label: "UAT",
    icon: "Clipboard",
    color: "orange.400",
  },
  {
    value: "Production",
    label: "Production",
    icon: "BarChart2",
    color: "green.400",
  },
];
