"use client";

import { ColumnDef } from "@tanstack/react-table";
import { StudyPartial } from "@/types/studies";
import { DataTableColumnHeader } from "@/components/data-table/column-header";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import { MoreHorizontal } from "lucide-react";
import { visuallyHidden } from "@/styled-system/patterns";

export const columns: ColumnDef<StudyPartial>[] = [
  {
    accessorKey: "redCapId",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Red Cap ID" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "name",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Study Name" />
    ),
  },
  {
    id: "actions",
    cell: ({ row }) => {
      // TODO: Add editing / deleting actions here.

      return (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost">
              <span className={visuallyHidden()}>Open menu</span>
              <MoreHorizontal />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>Actions</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem>Edit</DropdownMenuItem>
            <DropdownMenuItem>Delete</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    },
  },
];
