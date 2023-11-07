import { getStudy } from "@/lib/api/studies";
import { UpdateForm } from "@/components/studies/Update";
import { Metadata } from "next";
import { container } from "@/styled-system/patterns";

export async function generateMetadata({
  params,
}: {
  params: { id: number };
}): Promise<Metadata> {
  const { id } = params;
  const study = await getStudy(id);

  return {
    title: `Edit Study: ${study.name}`,
  };
}

export default async function EditPage({
  params: { id },
}: {
  params: { id: number };
}) {
  const study = await getStudy(id);

  return (
    <div
      className={container({
        maxWidth: { base: "8xl", md: "2/3" },
        marginTop: "12",
      })}
    >
      <UpdateForm model={study} />
    </div>
  );
}
