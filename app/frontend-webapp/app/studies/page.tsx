import { getStudies } from "@/api/studies";

import { Metadata } from "next";
import { DataTable } from "@/components/data-table";
import { container, flex } from "@/styled-system/patterns";
import { columns } from "./columns";
import AddStudy from "@/components/studies/Add";
import { h1 } from "@/styled-system/recipes";
import { environments } from "@/constants/environments";

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
    <div className={container({ maxWidth: { base: "8xl" } })}>
      <div className={flex({ gap: "6", direction: "column" })}>
        <h1 className={h1()}>RedCap Studies</h1>
        <DataTable columns={columns} data={studies} facets={facets}>
          <AddStudy />
        </DataTable>
      </div>
    </div>
  );
}
