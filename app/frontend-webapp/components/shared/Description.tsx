import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "../ui/tooltip";

interface DescriptionProps {
  text: string;
  children: React.ReactElement;
}

export function Description({ text, children }: DescriptionProps) {
  return (
    <TooltipProvider>
      <Tooltip>
        <TooltipTrigger asChild>{children}</TooltipTrigger>
        <TooltipContent className="text-white bg-black dark:text-black dark:bg-white">
          <h1>{text}</h1>
        </TooltipContent>
      </Tooltip>
    </TooltipProvider>
  );
}
