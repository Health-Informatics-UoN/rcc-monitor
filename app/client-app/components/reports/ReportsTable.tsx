"use client";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
  } from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Box } from "@/styled-system/jsx";
import { css } from "@/styled-system/css";
import { useState } from "react";

export default function ReportsTable({ reports }: { reports: ReportModel[] }) {
  const [filterValue, setFilterValue] = useState("");

  const filteredReports = reports.filter((report) => {
    return (
      report.dateTime.toString().includes(filterValue) ||
      report.description.includes(filterValue) ||
      report.status.name.includes(filterValue)
    );
  });

  const handleFilterChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilterValue(event.target.value);
  };

  return (
    <Box w="50%">
      <Input
        placeholder="Filter..."
        maxW="sm"
        m="0px auto 10px 0px"
        onChange={handleFilterChange}
        value={filterValue}
      />
      <Table m="10px auto">
        <TableHeader>
          <TableRow>
            <TableHead>Date Time</TableHead>
            <TableHead>Description</TableHead>
            <TableHead>Status</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {filteredReports.length <= 0 && (
            <h3 className={css({ fontWeight: "bold", m: "20px" })}>
              No results found
            </h3>
          )}
          {filteredReports.map((report) => (
            <TableRow key={report.id}>
              <TableCell>{report.dateTime.toString()}</TableCell>
              <TableCell>{report.description}</TableCell>
              <TableCell>{report.status.name}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </Box>
  );
}
