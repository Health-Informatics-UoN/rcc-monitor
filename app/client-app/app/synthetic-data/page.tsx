import { Metadata } from "next";
import { postSpreadsheet } from "@/lib/api/syntheticdata";
import { AddForm } from "./form";

export const metadata: Metadata = {
  title: "RedCap Site Reports",
};

export default async function Page() {
  return (
    <>
      <AddForm />
    </>
  );
}
