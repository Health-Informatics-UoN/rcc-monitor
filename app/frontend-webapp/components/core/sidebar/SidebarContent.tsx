import { getServerSession } from "next-auth";

import { getFeatureFlags } from "@/api/config";
import { options } from "@/lib/auth";
import { isUserAuthorized } from "@/lib/auth";

import { Brand } from "./Brand";
import { SidebarButton, SidebarItem } from "./SidebarItem";

export const SidebarContent = async ({ items }: { items: SidebarItem[] }) => {
  const session = await getServerSession(options);
  const flags = await getFeatureFlags();

  return (
    <>
      <Brand />
      <div className="flex flex-col gap-1 items-start mt-6 pl-4 pr-4">
        {items?.map((item, i) => {
          // Check if the user has permission to see this item
          const authorised =
            !item.policy || isUserAuthorized(session?.token, item.policy);

          // Check if the feature has been released
          const released = !item.featureFlag || flags[item.featureFlag];

          // Delete policy so the client doesn't error
          const itemWithoutPolicy = { ...item };
          delete itemWithoutPolicy.policy;

          return authorised && released ? (
            <SidebarButton item={itemWithoutPolicy} key={i} />
          ) : null;
        })}
      </div>
    </>
  );
};
