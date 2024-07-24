import { Icons } from "@/components/shared/Icons";
import { environments } from "@/constants/environments";

import { Badge } from "../ui/badge";

export default function EnvironmentBadge({ name }: { name: string }) {
  const environment = environments.find((e) => e.value === name);

  if (!environment) {
    return null;
  }

  const Icon = Icons[environment.icon];

  const facetColorVariants: {
    [key: string]: string;
  } = {
    Build: "bg-red-400 hover:bg-red-400",
    UAT: "bg-orange-400 hover:bg-orange-400",
    Production: "bg-green-400 hover:bg-green-400",
  };

  return (
    <Badge className={`${facetColorVariants[environment.value]} rounded-sm`}>
      {environment.icon && <Icon className={`icon-md mr-2`} />}
      <span>{environment.label}</span>
    </Badge>
  );
}
