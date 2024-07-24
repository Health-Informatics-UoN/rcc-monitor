import { Column } from "@tanstack/react-table";
import { CheckIcon, PlusCircleIcon } from "lucide-react";
import * as React from "react";

import { FacetsFilter, Icons } from "@/components/shared/Icons";

import { Badge } from "../ui/badge";
import { Button } from "../ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "../ui/command";
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover";
import { Separator } from "../ui/separator";

interface DataTableFacetedFilterProps<TData, TValue> {
  column?: Column<TData, TValue>;
  title?: string;
  options: FacetsFilter[];
}

export function DataTableFacetedFilter<TData, TValue>({
  column,
  title,
  options,
}: DataTableFacetedFilterProps<TData, TValue>) {
  const facets = column?.getFacetedUniqueValues();
  const selectedValues = new Set(column?.getFilterValue() as string[]);
  const facetColorVariants: {
    [key: string]: string;
  } = {
    Build: "bg-red-400 hover:bg-red-400",
    UAT: "bg-orange-400 hover:bg-orange-400",
    Production: "bg-green-400 hover:bg-green-400",
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="outline">
          {title}
          {selectedValues?.size > 0 && (
            <>
              <Separator orientation="vertical" />
              <Badge variant="secondary">{selectedValues.size}</Badge>
              <div className="hidden mr-1 ml-1 lg:flex">
                {selectedValues.size > 2 ? (
                  <Badge variant="secondary" className="rounded-sm">
                    {selectedValues.size} selected
                  </Badge>
                ) : (
                  options
                    .filter((option) => selectedValues.has(option.value))
                    .map((option) => (
                      <Badge
                        className={`${facetColorVariants[option.value]} rounded-sm mx-1`}
                        key={option.value}
                      >
                        {option.label}
                      </Badge>
                    ))
                )}
              </div>
            </>
          )}
          <PlusCircleIcon className={`icon-md ml-2`} />
        </Button>
      </PopoverTrigger>
      <PopoverContent>
        <Command>
          <CommandInput placeholder={title} />
          <CommandList>
            <CommandEmpty>No results found.</CommandEmpty>
            <CommandGroup>
              {options.map((option) => {
                const isSelected = selectedValues.has(option.value);

                const Icon = Icons[option.icon];
                return (
                  <CommandItem
                    key={option.value}
                    onSelect={() => {
                      if (isSelected) {
                        selectedValues.delete(option.value);
                      } else {
                        selectedValues.add(option.value);
                      }
                      const filterValues = Array.from(selectedValues);
                      column?.setFilterValue(
                        filterValues.length ? filterValues : undefined
                      );
                    }}
                  >
                    <div className="mr-2 flex h-4 w-4 align-center justify-center rounded-sm border">
                      {isSelected && <CheckIcon className={`icon-lg`} />}
                    </div>

                    {option.icon && <Icon className={`icon-md mr-2`} />}

                    <span>{option.label}</span>
                    {facets?.get(option.value) && (
                      <span>{facets.get(option.value)}</span>
                    )}
                  </CommandItem>
                );
              })}
            </CommandGroup>
            {selectedValues.size > 0 && (
              <>
                <CommandSeparator />
                <CommandGroup>
                  <CommandItem
                    onSelect={() => column?.setFilterValue(undefined)}
                    className="justify-center text-center"
                  >
                    Clear filters
                  </CommandItem>
                </CommandGroup>
              </>
            )}
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
