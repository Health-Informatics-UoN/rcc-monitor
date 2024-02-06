import { getServerSession } from "next-auth";

import { getFeatureFlags } from "@/api/config";
import { Brand } from "@/components/sidebar/Brand";
import { SidebarButton } from "@/components/sidebar/SidebarItem";
import { options } from "@/lib/auth";
import { isUserAuthorized } from "@/lib/auth";
import { vstack } from "@/styled-system/patterns";
import { SidebarItem } from "@/types";

export const SidebarContent = async ({ items }: { items: SidebarItem[] }) => {
  const session = await getServerSession(options);
  const flags = await getFeatureFlags();

  return (
    <>
      <Brand />
      <div
        className={vstack({
          gap: "1",
          alignItems: "start",
          mt: "6",
          pl: "4",
          pr: "4",
        })}
      >
        {items?.map((item, i) => {
          // Check if the user has permission to see this item
          const authorised =
            !item.policy || isUserAuthorized(session?.token, item.policy);

          // Check if the feature has been released
          const released = !item.featureFlag || flags[item.featureFlag];

          // Delete policy so the client doesn't error
          delete item.policy;

          return authorised && released ? (
            <SidebarButton item={item} key={i} />
          ) : null;
        })}
      </div>
    </>
  );
};
