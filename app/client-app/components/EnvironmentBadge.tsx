import { environments } from "@/constants/environments";
import { Icons } from "@/components/Icons";
import { Badge } from "@/components/ui/badge";
import { token } from "@/styled-system/tokens";
import { css } from "@/styled-system/css";
import { icon } from "@/styled-system/recipes";

export default function EnvironmentBadge({ name }: { name: string }) {
  const environment = environments.find((e) => e.value === name);

  if (!environment) {
    return null;
  }

  const Icon = Icons[environment.icon];

  return (
    <Badge
      style={{
        background: token.var(`colors.${environment.color}`),
      }}
      className={css({
        rounded: "sm",
      })}
    >
      {environment.icon && <Icon className={icon({ right: "sm" })} />}
      <span>{environment.label}</span>
    </Badge>
  );
}
