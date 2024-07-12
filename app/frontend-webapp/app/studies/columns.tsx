"use client";

import { ColumnDef } from "@tanstack/react-table";
import {
  AlertCircle,
  ChevronRightIcon,
  Eye,
  MoreHorizontal,
  XIcon,
} from "lucide-react";
import Link from "next/link";
import { useSession } from "next-auth/react";
import { useRef } from "react";

import { deleteStudy } from "@/api/studies";
import { AuthorizationPolicies } from "@/auth/AuthPolicies";
import { DataTableColumnHeader } from "@/components/data-table/DataTableColumnHeader";
import { ConfirmationDialog } from "@/components/shared/ConfirmationDialog";
import EnvironmentBadge from "@/components/shared/EnvironmentBadge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { useToast } from "@/components/ui/use-toast";
import { redCapBuildUrl, redCapProductionUrl, redCapUatUrl } from "@/constants";
import { isUserAuthorized } from "@/lib/auth";
import { StudyPartial } from "@/types/studies";

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
    id: "status",
    enableHiding: true,
    enableSorting: true,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
    cell: ({ row }) => {
      const alerts = [
        {
          isActive: row.original.studyCapacityAlert,
          message: "Study has reached its capacity threshold.",
        },
        {
          isActive: row.original.productionSubjectsEnteredAlert,
          message:
            "Production Subjects check has reached the data threshold, real data might be entered on a Build study.",
        },
      ];

      const activeAlerts = alerts.filter((alert) => alert.isActive);

      if (activeAlerts.length === 0) {
        return null;
      }

      return (
        <div className="flex justify-center items-center">
          {activeAlerts.map((alert, index) => (
            <Tooltip key={index}>
              <TooltipTrigger asChild>
                <AlertCircle className="h-5 w-5 text-red-500" />
              </TooltipTrigger>
              <TooltipContent>{alert.message}</TooltipContent>
            </Tooltip>
          ))}
        </div>
      );
    },
  },
  {
    id: "actions",
    cell: ({ row }) => {
      const study = row.original;
      const deleteButtonRef = useRef<HTMLButtonElement | null>(null);
      const { data: session } = useSession();
      const { toast } = useToast();

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
              <span className="absolute w-1 h-1 p-0 m-n1 overflow-hidden whitespace-nowrap border-0">
                Open menu
              </span>
              <MoreHorizontal className={`icon-md`} />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>Actions</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem>
              <ChevronRightIcon className={`icon-md`} />
              <a href={redCapLink} target="blank">
                View on RedCap
              </a>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <Link href={`/studies/${study.id}`}>
              <DropdownMenuItem>
                <Eye className={`icon-md`} />
                View
              </DropdownMenuItem>
            </Link>

            {isUserAuthorized(
              session?.token,
              AuthorizationPolicies.CanDeleteStudies
            ) && (
              <DropdownMenuItem
                onClick={(e: React.MouseEvent<HTMLElement>) => {
                  e.preventDefault();
                  deleteButtonRef.current?.click();
                }}
              >
                <XIcon className={`icon-md`} />
                Delete
              </DropdownMenuItem>
            )}

            <div className="absolute w-1 h-1 p-0 m-n1 overflow-hidden whitespace-nowrap border-0">
              <ConfirmationDialog
                title="Are you sure you want to delete this study?"
                description="This action cannot be undone"
                leftButtonName="Cancel"
                rightButtonName="Delete"
                refProp={deleteButtonRef}
                handleClick={() => handleDelete(study.id)}
                css="bg-red-500"
              />
            </div>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    },
  },
];
