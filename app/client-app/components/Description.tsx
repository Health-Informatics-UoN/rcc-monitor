import {
  HoverCard,
  HoverCardContent,
  HoverCardTrigger,
} from "@/components/ui/hover-card";

interface DescriptionProps {
  text: string;
  children: React.ReactElement;
}

export function Description({ text, children }: DescriptionProps) {
  return (
    <HoverCard>
      <HoverCardTrigger asChild>{children}</HoverCardTrigger>
      <HoverCardContent>
        <h1>{text}</h1>
      </HoverCardContent>
    </HoverCard>
  );
}
