"use client";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Box } from "@/styled-system/jsx";
import { css } from "@/styled-system/css";
import { useSortingAndFiltering } from "@/lib/hooks/useSortingAndFiltering";

interface ReportRow {
  dateTime: string;
  status: string;
  instance?: string;
  parentId?: string;
  siteId?: string;
  siteName?: string;
  parentInBuild?: string;
  parentIdInProduction?: string;
  parentIdInUAT?: string;
  siteNameInBuild?: string;
  siteNameInProduction?: string;
  siteNameInUAT?: string;
}

interface ReportsTableProps {
  columns: string[];
  rows: ReportRow[];
}

function toCamelCase(str: string) {
  let ans = str.toLowerCase();
  return ans.split(" ").reduce((s, c) => s + (c.charAt(0).toUpperCase() + c.slice(1)));
}

export default function ReportsTable({ columns, rows }: ReportsTableProps) {
  const { onSort, filter, setFilter, outputList } = useSortingAndFiltering(
    rows,
    "siteName",
    {
      initialSort: { key: "dateTime", asc: true },
      sorters: {
        dateTime: { sorter: (asc) => (a, b) => asc ? a - b : b - a }
      },
      storageKey: "reportsTableSorting"
    }
  );

  return (
    <Box>
      <Input
        placeholder="Filter..."
        maxW="sm"
        m="0px auto 10px 0px"
        value={filter}
        onChange={(e) => setFilter(e.target.value)}
      />
      <Table m="10px auto">
        <TableHeader>
          <TableRow>
            {columns.map((column, index) => (
              <TableHead key={index} onClick={() => onSort(toCamelCase(column))}>
                {column}
              </TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {outputList.length === 0 && (
            <h3 className={css({ fontWeight: "bold", m: "20px" })}>
              No results found
            </h3>
          )}
          {outputList.map((row, index) => (
            <TableRow key={index}>
              {Object.values(row).map((cell: string, index: number) => (
                <TableCell key={index}>
                  {cell}
                </TableCell>
              ))}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </Box>
  );
}
