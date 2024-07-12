"use client";

import { usePathname } from "next/navigation";

import { Icons } from "@/components/shared/Icons";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { button } from "@/components/ui/button";
import { AuthorizationPolicy } from "@/lib/auth";
import { FeatureFlagModel } from "@/types/config";

interface SidebarLink {
  name: string;
  path?: string;
  icon?: keyof typeof Icons;
}

export interface SidebarItem extends SidebarLink {
  policy?: AuthorizationPolicy;
  featureFlag?: keyof FeatureFlagModel;
  children?: SidebarLink[];
}

export const SidebarButton = ({ item }: { item: SidebarItem }) => {
  if (item.children) {
    return <SidebarAccordion item={item} />;
  }

  if (!item.path) {
    return <SidebarTitle item={item} />;
  }

  return <SidebarLinkButton item={item} />;
};

const SidebarLinkButton = ({ item }: { item: SidebarItem }) => {
  const pathname = usePathname();
  const Icon = item.icon ? Icons[item.icon] : null;
  return (
    <a
      key={item.name}
      href={item.path}
      aria-current={
        item.path && pathname.startsWith(item.path) ? "page" : undefined
      }
      className={`${button("ghost")} w-full justify-left`}
    >
      {Icon && <Icon className="icon-md mr-4" />}
      {item.name}
    </a>
  );
};

const SidebarTitle = ({ item }: { item: SidebarLink }) => {
  const Icon = item.icon ? Icons[item.icon] : null;
  return (
    <div className="flex items-center">
      {Icon && <Icon className="icon-md ml-2" />}
      <h2 className="text-lg font-semibold tracking-tight py-1">{item.name}</h2>
    </div>
  );
};

const SidebarAccordion = ({ item }: { item: SidebarItem }) => {
  return (
    <Accordion type="single" collapsible className="w-full">
      <AccordionItem value="item-1" className="border-none">
        <AccordionTrigger
          className={`${button("ghost")} w-full justify-left hover:no-underline`}
        >
          <SidebarTitle item={item} />
        </AccordionTrigger>
        <AccordionContent className="px-4">
          {item.children?.map((child, i) => {
            return <SidebarLinkButton item={child} key={i} />;
          })}
        </AccordionContent>
      </AccordionItem>
    </Accordion>
  );
};
