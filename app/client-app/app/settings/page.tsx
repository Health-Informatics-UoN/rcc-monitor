import { css } from "@/styled-system/css";
import { Box } from "@/styled-system/jsx";
import Config from "./config";
import { getSiteConfig } from "@/lib/api/config";

export default async function Page() {
  const config = await getSiteConfig();

  return (
    <div>
      <Box>
        <h1
          className={css({
            fontSize: "2rem",
            fontWeight: "bold",
            m: "20px 0px",
          })}
        >
          Settings
        </h1>
      </Box>
      {config.map((config) => (
        <Config key={config.key} config={config} />
      ))}
    </div>
  );
}
