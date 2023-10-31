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
import { ConfirmationDialog } from "@/components/ConfirmationDialog";
import { useRef } from "react";
import { deleteStudy } from "@/lib/api/studies";
import { toast } from "@/components/ui/toast/use-toast";

export const columns: ColumnDef<StudyPartial>[] = [
  {
    accessorKey: "id",
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
      const study = row.original;
      // TODO: Add editing actions here.
      const deleteButtonRef = useRef<HTMLButtonElement | null>(null);

      const handleDelete = async (id: number) => {
        const response = await deleteStudy(id);
        if (!response.success) {
          toast({
            variant: "destructive",
            title: "Failed to delete study!",
            description: response.message,
          });
          return;
        }
        toast({
          title: "Study successfully deleted!",
        });
      };

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
            <DropdownMenuItem
              onClick={(e: React.MouseEvent<HTMLElement>) => {
                e.preventDefault();
                deleteButtonRef.current?.click();
              }}
            >
              Delete
            </DropdownMenuItem>
            <div className={visuallyHidden()}>
              <ConfirmationDialog
                title="Are you sure you want to delete this study?"
                description="This action cannot be undone"
                leftButtonName="Cancel"
                rightButtonName="Delete"
                refProp={deleteButtonRef}
                handleClick={() => handleDelete(study.id)}
                css={{ backgroundColor: "red" }}
              />
            </div>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    },
  },
];
