import { useState, useEffect } from "react";

/**
 * Gets the appropriate sort function for a given object property.
 *
 * @param {string} key The object property to sort by.
 * @param {boolean} asc Sort ascending or descending.
 * @param {object} sorters Custom sorting behaviours.
 *
 * It's possible to specify for a given sorting key an alternative object property to sort against,
 * or alternative sorting logic as a sorting predicate (asc) => (a, b) => {}
 *
 * example:
 * {
 *   active: { property: "activeInstanceId" },
 *   name: {sorter: asc => (a, b) => asc ? a.localeCompare(b) : b.localeCompare(a) }
 * }
 *
 */

interface Sorters {
  [key: string]: {
    property?: string;
    sorter?: (asc: boolean) => (a: any, b: any) => number;
  };
}

interface SortingOptions {
  key: string;
  asc: boolean;
}

interface SortingFilteringOptions {
  initialSort: SortingOptions;
  sorters?: Sorters;
  storageKey: string;
}

const getPropertySorter = (
  key: string,
  asc: boolean,
  sorters: Sorters = {}
) => {
  const customSorter = sorters[key] || {};

  const { property, sorter } = customSorter;
  return ({ [property ?? key]: a }: any, { [property ?? key]: b }: any) => {
    if (sorter) {
      return sorter(asc)(a, b);
    } else {
      return asc ? a - b : b - a;
    }
  };
};

/**
 * Build a lookup list of object IDs and names
 * sorted by the specified object property,
 * ascending or descending.
 *
 * @param {object} input The source dictionary of objects.
 * @param {string} key The object property to sort by.
 * @param {boolean} asc Sort ascending or descending.
 */
const getSortedLookup = (
  input: Record<string, any>,
  key: string,
  asc: boolean,
  sorters?: Sorters
) =>
  Object.keys(input)
    .map((id) => input[id])
    .sort(getPropertySorter(key, asc, sorters));

/**
 * Filters a list of objects with a `name` (or custom specified property name) property
 * where said property contains a match on the `filter` string.
 *
 * If no filter is specified, the full input list is returned.
 *
 * @param {object[]} input The source list of objects.
 * @param {string} filter The filter string to match against.
 * @param {string|function} filterer
 * If a string, is a property name used to match against filter
 *
 * If a function, is executed to perform custom filtering, with the signature (input, filter) => {}
 */
const getFilteredLookup = (
  input: any[],
  filter: string,
  filterer: string | ((value: any, filter: string) => boolean) = "name"
) =>
  !filter
    ? input
    : input.filter(
        typeof filterer === "string"
          ? ({ [filterer]: keyProperty }: any) =>
              new RegExp(filter, "i").test(keyProperty)
          : (value) => filterer(value, filter)
      );

const storageKeyPrefix = "redCapSiteReports.sorting";

export const useSortingAndFiltering = (
  sourceList: Record<string, any>[],
  filterer: string | ((value: any, filter: string) => boolean),
  { initialSort, sorters, storageKey }: SortingFilteringOptions
) => {
  const storageKeyFull = `${storageKeyPrefix}.${storageKey}`;

  const [sorting, setSorting] = useState<SortingOptions>(() => {
    const savedSorting = JSON.parse(
      localStorage.getItem(storageKeyFull) || "{}"
    );
    return {
      key: savedSorting.key || initialSort.key || "name",
      asc: savedSorting[initialSort.key] ?? initialSort.asc ?? true
    };
  });

  const [sortedList, setSortedList] = useState<Record<string, any>[]>([]);
  const [filter, setFilter] = useState<string>("");
  const [outputList, setOutputList] = useState<Record<string, any>[]>([]);

  // update the sorted list appropriately
  useEffect(() => {
    setSortedList(
      getSortedLookup(sourceList, sorting.key, sorting.asc, sorters)
    );
    // also save the new sort for next time
    localStorage.setItem(storageKeyFull, JSON.stringify(sorting));
  }, [sourceList, sorting]);

  // update the filtered list appropriately
  useEffect(() => {
    setOutputList(getFilteredLookup(sortedList, filter, filterer));
  }, [filter, sortedList]);

  // default sort handler
  const handleSort = (key: string) => {
    setSorting((prevSorting) => ({
      key,
      asc: prevSorting.key === key ? !prevSorting.asc : true
    }));
  };

  return {
    sorting,
    setSorting,
    onSort: handleSort,
    filter,
    setFilter,
    outputList
  };
};
