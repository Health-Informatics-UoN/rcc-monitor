"use client";

import { ColumnDef } from "@tanstack/react-table";
import { icon } from "@/styled-system/recipes";
import { css } from "@/styled-system/css";
import { center, visuallyHidden } from "@/styled-system/patterns";
import { DataTableColumnHeader } from "@/components/data-table/DataTableColumnHeader";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import {
  AlertCircle,
  ChevronRightIcon,
  Eye,
  MoreHorizontal,
  XIcon,
} from "lucide-react";
import { ConfirmationDialog } from "@/components/ConfirmationDialog";
import { useRef } from "react";
import { toast } from "@/components/ui/toast/use-toast";
import Link from "next/link";
import {
  HoverCard,
  HoverCardContent,
  HoverCardTrigger,
} from "@/components/ui/hover-card";
import { redCapBuildUrl, redCapProductionUrl, redCapUatUrl } from "@/constants";
import { deleteStudy } from "@/lib/api/studies";
import { StudyPartial } from "@/types/studies";
import EnvironmentBadge from "@/components/EnvironmentBadge";

export const columns: ColumnDef<StudyPartial>[] = [
  {
    accessorKey: "id",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="RedCap ID" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "instance",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="RedCap Environment" />
    ),
    cell: ({ row }) => {
      return <EnvironmentBadge name={row.getValue("instance")} />;
    },
    enableColumnFilter: true,
    enableHiding: true,
    filterFn: (row, id, value) => {
      return value.includes(row.getValue(id));
    },
  },
  {
    accessorKey: "name",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Study Name" />
    ),
  },
  {
    accessorKey: "studyCapacityAlert",
    enableHiding: true,
    enableSorting: true,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
    cell: ({ row }) => {
      const alert = row.getValue("studyCapacityAlert");

      if (!alert) {
        return null;
      }

      return (
        <div className={center()}>
          <HoverCard>
            <HoverCardTrigger asChild>
              <AlertCircle
                className={css({
                  h: 5,
                  w: 5,
                  color: "red",
                })}
              />
            </HoverCardTrigger>
            <HoverCardContent>
              The Study has reached its capacity threshold.
            </HoverCardContent>
          </HoverCard>
        </div>
      );
    },
  },
  {
    id: "actions",
    cell: ({ row }) => {
      const study = row.original;
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

      let redCapLink;

      switch (study.instance) {
        case "Build":
          redCapLink = `${redCapBuildUrl}/#cid=nph2020&act=list&studyId=${study.id}`;
          break;
        case "Production":
          redCapLink = `${redCapProductionUrl}/#cid=nph2020&act=list&studyId=${study.id}`;
          break;
        case "UAT":
          redCapLink = `${redCapUatUrl}/#cid=nph2020&act=list&studyId=${study.id}`;
          break;
      }

      return (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost">
              <span className={visuallyHidden()}>Open menu</span>
              <MoreHorizontal className={icon({})} />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>Actions</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem>
              <ChevronRightIcon className={icon({})} />
              <a href={redCapLink} target="blank">
                View on RedCap
              </a>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <Link href={`/studies/${study.id}`}>
              <DropdownMenuItem>
                <Eye className={icon({})} />
                View
              </DropdownMenuItem>
            </Link>
            <DropdownMenuItem
              onClick={(e: React.MouseEvent<HTMLElement>) => {
                e.preventDefault();
                deleteButtonRef.current?.click();
              }}
            >
              <XIcon className={icon({})} />
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
