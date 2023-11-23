import {
  EyeOffIcon,
  ChevronDownIcon,
  ChevronUpIcon,
  ChevronsUpDownIcon,
} from "lucide-react";
import { Column } from "@tanstack/react-table";
import { css } from "@/styled-system/css";
import { Flex } from "@/styled-system/jsx";
import { icon } from "@/styled-system/recipes";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

interface DataTableColumnHeaderProps<TData, TValue>
  extends React.HTMLAttributes<HTMLDivElement> {
  column: Column<TData, TValue>;
  title: string;
}

export function DataTableColumnHeader<TData, TValue>({
  column,
  title,
  className,
}: DataTableColumnHeaderProps<TData, TValue>) {
  if (!column.getCanSort()) {
    return <div className={className}>{title}</div>;
  }

  return (
    <Flex align="center" gap="2">
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button
            variant="ghost"
            size="sm"
            className={css({
              ml: "-3",
              h: "8",
              '&[data-state="open]': {
                bg: "accent",
              },
            })}
          >
            <span>{title}</span>
            {column.getIsSorted() === "desc" ? (
              <ChevronDownIcon className={icon({ left: "sm" })} />
            ) : column.getIsSorted() === "asc" ? (
              <ChevronUpIcon className={icon({ left: "sm" })} />
            ) : (
              <ChevronsUpDownIcon className={icon({ left: "sm" })} />
            )}
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="start">
          <DropdownMenuItem onClick={() => column.toggleSorting(false)}>
            <ChevronUpIcon className={icon({})} />
            Asc
          </DropdownMenuItem>
          <DropdownMenuItem onClick={() => column.toggleSorting(true)}>
            <ChevronDownIcon className={icon({})} />
            Desc
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem onClick={() => column.toggleVisibility(false)}>
            <EyeOffIcon className={icon({})} />
            Hide
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </Flex>
  );
}
