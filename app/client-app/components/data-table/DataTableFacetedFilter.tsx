import * as React from "react";
import { Column } from "@tanstack/react-table";
import { PlusCircleIcon, CheckIcon } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Separator } from "@/components/ui/separator";
import { icon } from "@/styled-system/recipes";
import { FacetsFilter, Icons } from "@/components/Icons";
import { css } from "@/styled-system/css";
import { token } from "@/styled-system/tokens";

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

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="outline">
          {title}
          {selectedValues?.size > 0 && (
            <>
              <Separator orientation="vertical" />
              <Badge variant="secondary">{selectedValues.size}</Badge>
              <div
                className={css({
                  display: "none",
                  mr: "1",
                  ml: "1",
                  lg: { display: "flex" },
                })}
              >
                {selectedValues.size > 2 ? (
                  <Badge
                    variant="secondary"
                    className={css({
                      rounded: "sm",
                    })}
                  >
                    {selectedValues.size} selected
                  </Badge>
                ) : (
                  options
                    .filter((option) => selectedValues.has(option.value))
                    .map((option) => (
                      <Badge
                        style={{
                          background: token.var(`colors.${option.color}`),
                        }}
                        className={css({
                          rounded: "sm",
                          mx: 1,
                        })}
                        key={option.value}
                      >
                        {option.label}
                      </Badge>
                    ))
                )}
              </div>
            </>
          )}
          <PlusCircleIcon className={icon({ right: "sm" })} />
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
                    <div
                      className={css({
                        mr: "2",
                        display: "flex",
                        h: "4",
                        w: "4",
                        alignItems: "center",
                        justifyContent: "center",
                        rounded: "sm",
                        borderWidth: "1px",
                      })}
                    >
                      {isSelected && (
                        <CheckIcon className={icon({ size: "lg" })} />
                      )}
                    </div>

                    {option.icon && <Icon className={icon({ right: "sm" })} />}

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
