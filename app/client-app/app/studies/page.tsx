import { getStudies } from "@/lib/api/studies";
import { css } from "@/styled-system/css";
import { Metadata } from "next";
import { DataTable } from "@/components/data-table";
import { container, flex } from "@/styled-system/patterns";
import { columns } from "./columns";
import AddStudy from "@/components/studies/add";

export const metadata: Metadata = {
  title: "RedCap Studies",
};

export default async function Studies() {
  const studies = await getStudies();

  return (
    <div className={container({ maxWidth: { base: "8xl", md: "2/3" } })}>
      <div className={flex({ gap: "6", direction: "column", mt: "6" })}>
        <h1
          className={css({
            fontSize: "35px",
            fontWeight: "bold",
            textAlign: "center",
          })}
        >
          RedCap Studies
        </h1>
        <DataTable columns={columns} data={studies}>
          <AddStudy />
        </DataTable>
      </div>
    </div>
  );
}
