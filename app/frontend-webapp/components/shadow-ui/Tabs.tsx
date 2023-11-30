"use client";

import * as TabsPrimitive from "@radix-ui/react-tabs";
import { createStyleContext } from "@shadow-panda/style-context";
import { styled } from "@/styled-system/jsx";
import { tabs } from "@/styled-system/recipes";
import { css } from "@/styled-system/css";

const { withProvider, withContext } = createStyleContext(tabs);

export const triggerStyle = css({
  position: "relative",
  h: "9",
  rounded: "0",
  borderBottom: "2px solid transparent",
  bg: "transparent",
  px: "4",
  pb: "3",
  pt: "2",
  fontWeight: "semibold",
  color: "muted.foreground",
  shadow: "none",
  transition: "none",
  cursor: "pointer",

  "&[data-state=active]": {
    borderBottomColor: "primary",
    color: "foreground",
    shadow: "none",
  },
});

export const Tabs = withProvider(styled(TabsPrimitive.Root), "root");
export const TabsList = withContext(styled(TabsPrimitive.List), "list");
export const TabsTrigger = withContext(
  styled(TabsPrimitive.Trigger),
  "trigger"
);
export const TabsContent = withContext(
  styled(TabsPrimitive.Content),
  "content"
);
