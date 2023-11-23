"use client";

import { Table } from "@tanstack/react-table";
import { Flex } from "@/styled-system/jsx";

import { Input } from "@/components/ui/input";
import { DataTableFacetedProps } from "@/components/data-table";
import { DataTableFacetedFilter } from "@/components/data-table/DataTableFacetedFilter";
import { DataTableViewOptions } from "@/components/data-table/DataTableViewOptions";

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
    <Flex justifyContent={"space-between"}>
      <Flex gap={"4"}>
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
      </Flex>

      <Flex gap={"4"}>
        {children}
        <DataTableViewOptions table={table} />
      </Flex>
    </Flex>
  );
}
