import { Metadata } from "next";

import { getStudies } from "@/api/studies";
import { DataTable } from "@/components/data-table";
import AddStudy from "@/components/studies/Add";
import { environments } from "@/constants/environments";

import { columns } from "./columns";

export const metadata: Metadata = {
  title: "RedCap Studies",
};

export default async function Studies() {
  const studies = await getStudies();

  const facets = [
    {
      column: "instance",
      title: "Environment",
      options: environments,
    },
  ];

  return (
    <div className="container mx-auto max-w-full">
      <div className="flex flex-col gap-6">
        <h1 className={`h1`}>RedCap Studies</h1>
        <DataTable columns={columns} data={studies} facets={facets}>
          <AddStudy />
        </DataTable>
      </div>
    </div>
  );
}
