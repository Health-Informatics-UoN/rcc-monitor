import { getStudy } from "@/api/studies";
import { DetailsPage } from "@/components/studies/Details";
import { Metadata } from "next";
import { getSiteConfig } from "@/api/config";

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
    <div className="relative mx-auto mt-12 max-w-8xl md:max-w-2/3 md:px-6 lg:px-8">
      <DetailsPage model={study} config={config} />
    </div>
  );
}
