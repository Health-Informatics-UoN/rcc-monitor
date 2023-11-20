import {
  HoverCard,
  HoverCardContent,
  HoverCardTrigger,
} from "@/components/ui/hover-card";

interface ConfigDescriptionProps {
  description: string;
  children: React.ReactElement;
}

export function ConfigDescription({
  description,
  children,
}: ConfigDescriptionProps) {
  return (
    <HoverCard>
      <HoverCardTrigger asChild>{children}</HoverCardTrigger>
      <HoverCardContent>
        <h1>{description}</h1>
      </HoverCardContent>
    </HoverCard>
  );
}
