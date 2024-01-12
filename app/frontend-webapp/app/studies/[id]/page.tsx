import { Metadata } from "next";

import { getSiteConfig } from "@/api/config";
import { getStudy } from "@/api/studies";
import { DetailsPage } from "@/components/studies/Details";
import { container } from "@/styled-system/patterns";

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
    <div>
      <DetailsPage model={study} config={config} />
    </div>
  );
}
