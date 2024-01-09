"use client";

import { ColumnDef } from "@tanstack/react-table";
import { format } from "date-fns";

import { DataTableColumnHeader } from "@/components/data-table/DataTableColumnHeader";
import { ReportModel } from "@/types";

export const columns: ColumnDef<ReportModel>[] = [
  {
    id: "dateTime",
    accessorFn: (row) => format(new Date(row.dateTime), "dd-MM-yyyy"),
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Date Occurred" />
    ),
    enableHiding: false,
  },
  {
    id: "siteId",
    accessorFn: (row) => `${row.sites[0].siteId}`,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Id" />
    ),
    enableHiding: false,
  },
  {
    id: "name",
    accessorFn: (row) => `${row.sites[0].siteName}`,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Name" />
    ),
    enableHiding: false,
  },
];

export const parentSiteIdConflictColumns: ColumnDef<ReportModel>[] = [
  ...columns,
  {
    id: "parentSiteId",
    accessorFn: (row) => `${row.sites[0].parentSiteId}`,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Parent Site Id" />
    ),
    enableHiding: false,
  },
];
