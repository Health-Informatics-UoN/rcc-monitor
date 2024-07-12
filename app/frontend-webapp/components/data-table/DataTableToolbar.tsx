"use client";

import { Input } from "../ui/input";
import { DataTableFacetedProps } from ".";
import { Table } from "@tanstack/react-table";
import { DataTableViewOptions } from "./DataTableViewOptions";
import { DataTableFacetedFilter } from "./DataTableFacetedFilter";

interface DataTableToolbarProps<TData>
  extends React.HTMLAttributes<HTMLDivElement> {
  table: Table<TData>;
  facets?: DataTableFacetedProps[];
}

export function DataTableToolbar<TData>({
  table,
  facets,
  children,
}: DataTableToolbarProps<TData>) {
  return (
    <div className="flex justify-between">
      <div className="flex gap-4">
        <Input
          placeholder="Filter..."
          value={(table.getColumn("name")?.getFilterValue() as string) ?? ""}
          onChange={(event) =>
            table.getColumn("name")?.setFilterValue(event.target.value)
          }
        />

        {facets?.map((facet) => {
          return (
            <DataTableFacetedFilter
              key={facet.column}
              column={table.getColumn(facet.column)}
              title={facet.title}
              options={facet.options}
            />
          );
        })}
      </div>

      <div className="flex gap-4">
        {children}
        <DataTableViewOptions table={table} />
      </div>
    </div>
  );
}
