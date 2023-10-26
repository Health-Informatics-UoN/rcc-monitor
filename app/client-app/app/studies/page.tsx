import { getStudies } from "@/lib/api/studies";
import { css } from "@/styled-system/css";
import { Metadata } from "next";
import { DataTable } from "@/components/data-table";
import { columns } from "./columns";
import { container } from "@/styled-system/patterns";

export const metadata: Metadata = {
  title: "RedCap Studies",
};

export default async function Studies() {
  const studies = await getStudies();

  return (
    <div className={container({ maxWidth: { base: "8xl", md: "2/3" } })}>
      <h1
        className={css({
          mt: "10px",
          fontSize: "35px",
          fontWeight: "bold",
          textAlign: "center",
        })}
      >
        RedCap Studies
      </h1>
      <DataTable columns={columns} data={studies} />
    </div>
  );
}
