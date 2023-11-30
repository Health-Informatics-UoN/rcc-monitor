import { Box } from "@/styled-system/jsx";
import Config from "./config";
import { getSiteConfig } from "@/api/config";
import { h1 } from "@/styled-system/recipes";

export default async function Page() {
  const config = await getSiteConfig();

  return (
    <div>
      <Box>
        <h1 className={h1()}>Settings</h1>
      </Box>
      {config.map((config) => (
        <Config key={config.key} config={config} />
      ))}
    </div>
  );
}
