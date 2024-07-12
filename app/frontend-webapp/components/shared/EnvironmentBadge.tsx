import { Icons } from "@/components/shared/Icons";
import { environments } from "@/constants/environments";

import { Badge } from "../ui/badge";

export default function EnvironmentBadge({ name }: { name: string }) {
  const environment = environments.find((e) => e.value === name);

  if (!environment) {
    return null;
  }

  const Icon = Icons[environment.icon];

  return (
    <Badge className={`bg-${environment.color} rounded-sm`}>
      {environment.icon && <Icon className={`icon-md ml-2`} />}
      <span>{environment.label}</span>
    </Badge>
  );
}
