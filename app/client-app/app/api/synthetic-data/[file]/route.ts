import { getSpreadsheet } from "@/lib/api/syntheticdata";

export async function GET(
  request: Request,
  { params }: { params: { file: string } }
) {
  const f = await getSpreadsheet(params.file);
  // Create a new response with the blob
  const response = new Response(f);

  // Set the appropriate headers
  response.headers.set(
    "Content-Disposition",
    `attachment; filename="${params.file}"`
  );
  response.headers.set("Content-Type", "application/octet-stream");

  return response;
}
