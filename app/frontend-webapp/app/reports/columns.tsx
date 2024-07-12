"use client";
import { ColumnDef } from "@tanstack/react-table";

import { SiteReport } from "@/types/reports";

import { DataTableColumnHeader } from "../../components/data-table/DataTableColumnHeader";

const baseColumns: ColumnDef<SiteReport>[] = [
  {
    accessorKey: "dateOccured",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Date Occurred" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "lastChecked",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Last Checked" />
    ),
    enableHiding: false,
  },
];

export const siteConflictsColumns: ColumnDef<SiteReport>[] = [
  ...baseColumns,
  {
    accessorKey: "siteId",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Id" />
    ),
    enableHiding: false,
  },
  {
    id: "name",
    accessorFn: (row) => row.siteName,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Name" />
    ),
    enableHiding: false,
  },
];

export const siteNameConflictsColumns: ColumnDef<SiteReport>[] = [
  ...baseColumns,
  {
    accessorKey: "siteId",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Id" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "siteNameInBuild",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Name - Build" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "siteNameInProd",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Name - Production" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "siteNameInUAT",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Name - UAT" />
    ),
    enableHiding: false,
  },
];

export const siteParentIdConflictsColumns: ColumnDef<SiteReport>[] = [
  ...baseColumns,
  {
    id: "name",
    accessorFn: (row) => row.siteName,
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Site Name" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "parentSiteIdInBuild",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Parent Site Id - Build" />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "parentSiteIdInProd",
    header: ({ column }) => (
      <DataTableColumnHeader
        column={column}
        title="Parent Site Id - Production"
      />
    ),
    enableHiding: false,
  },
  {
    accessorKey: "parentSiteIdInUAT",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Parent Site Id - UAT" />
    ),
    enableHiding: false,
  },
];
