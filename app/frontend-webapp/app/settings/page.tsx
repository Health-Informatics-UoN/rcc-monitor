import { getSiteConfig } from "@/api/config";

import Config from "./config";

export default async function Page() {
  const config = await getSiteConfig();

  return (
    <div>
      <div>
        <h1 className={`h1`}>Settings</h1>
      </div>
      <div className="lg:w-[60%]">
        {config.map((config) => (
          <Config key={config.key} config={config} />
        ))}
      </div>
    </div>
  );
}
