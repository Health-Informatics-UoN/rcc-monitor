import { getStudy } from "@/lib/api/studies";
import { DetailsPage } from "@/components/studies/Details";
import { Metadata } from "next";
import { container } from "@/styled-system/patterns";
import { getSiteConfig } from "@/lib/api/config";

export async function generateMetadata({
  params,
}: {
  params: { id: number };
}): Promise<Metadata> {
  const { id } = params;
  const study = await getStudy(id);

  return {
    title: `${study.name}`,
  };
}

export default async function EditPage({
  params: { id },
}: {
  params: { id: number };
}) {
  const study = await getStudy(id);
  const config = await getSiteConfig();

  return (
    <div
      className={container({
        maxWidth: { base: "8xl", md: "2/3" },
        marginTop: "12",
      })}
    >
      <DetailsPage model={study} config={config} />
    </div>
  );
}
