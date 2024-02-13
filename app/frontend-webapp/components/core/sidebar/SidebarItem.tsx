"use client";

import { usePathname } from "next/navigation";

import { Icons } from "@/components/shared/Icons";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/shadow-ui/Accordion";
import { css, cx } from "@/styled-system/css";
import { button, icon } from "@/styled-system/recipes";
import { SidebarItem, SidebarLink } from "@/types";

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
      className={cx(
        button({ variant: "ghost" }),
        css({
          width: "100%",
          justifyContent: "left",
          _currentPage: {
            background: "gray.100",
            _dark: { background: "accent" },
          },
        })
      )}
    >
      {Icon && <Icon className={icon({ right: "sm" })} />}
      {item.name}
    </a>
  );
};

const SidebarTitle = ({ item }: { item: SidebarLink }) => {
  const Icon = item.icon ? Icons[item.icon] : null;
  return (
    <div
      className={css({
        display: "flex",
        alignItems: "center",
      })}
    >
      {Icon && <Icon className={icon({ right: "sm" })} />}
      <h2
        className={css({
          fontSize: "lg",
          fontWeight: "semibold",
          letterSpacing: "tight",
          py: "1",
        })}
      >
        {item.name}
      </h2>
    </div>
  );
};

const SidebarAccordion = ({ item }: { item: SidebarItem }) => {
  return (
    <Accordion
      type="single"
      collapsible
      className={css({
        width: "100%",
      })}
    >
      <AccordionItem
        value="item-1"
        className={css({
          border: "none",
        })}
      >
        <AccordionTrigger
          className={cx(
            button({ variant: "ghost" }),
            css({
              width: "100%",
              justifyContent: "left",
              _hover: {
                textDecoration: "none",
              },
            })
          )}
        >
          <SidebarTitle item={item} />
        </AccordionTrigger>
        <AccordionContent
          className={css({
            px: "4",
          })}
        >
          {item.children?.map((child, i) => {
            return <SidebarLinkButton item={child} key={i} />;
          })}
        </AccordionContent>
      </AccordionItem>
    </Accordion>
  );
};
