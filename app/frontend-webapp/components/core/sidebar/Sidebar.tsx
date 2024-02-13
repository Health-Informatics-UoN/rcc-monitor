import { MenuIcon } from "lucide-react";

import { UserMenu } from "@/components/core/UserMenu";
import {
  Sheet,
  SheetContent,
  SheetTrigger,
} from "@/components/shadow-ui/Sheet";
import { css } from "@/styled-system/css";
import { SidebarItem } from "@/types";

import { Brand } from "./Brand";
import { SidebarContent } from "./SidebarContent";

const MobileSidebar = ({
  children,
  sidebarItems,
}: {
  children: React.ReactNode;
  sidebarItems: SidebarItem[];
}) => {
  return (
    <>
      <Sheet side="left">
        <div
          className={css({
            mt: "5",
            mr: "8",
            ml: "6",
            zIndex: "10",
            display: "flex",
            md: { display: "none" },
            alignItems: "center",
          })}
        >
          <div className={css({ display: "flex", flex: "1" })}>
            <Brand />
          </div>
          <UserMenu />
          <SheetTrigger>
            <MenuIcon size={24} />
          </SheetTrigger>
        </div>

        <SheetContent>
          <SidebarContent items={sidebarItems} />
        </SheetContent>
      </Sheet>
      {children}
    </>
  );
};

const DesktopSidebar = ({
  children,
  sidebarItems,
}: {
  children: React.ReactNode;
  sidebarItems: SidebarItem[];
}) => {
  return (
    <div
      className={css({
        h: "screen",
        flex: "1",
        alignItems: "flex-start",
        overflowX: "hidden",
        md: {
          display: "grid",
          gridTemplateColumns: "200px minmax(0, 1fr)",
          gap: "6",
        },
        lg: {
          gridTemplateColumns: "260px minmax(0, 1fr)",
          gap: "10",
        },
      })}
    >
      <aside
        className={css({
          position: "sticky",
          top: "0",
          zIndex: "30",
          display: "none",
          h: "screen",
          w: "full",
          flexShrink: "0",
          borderRightWidth: "1px",
          borderRightColor: "gray.200",
          md: { pos: "sticky", display: "block" },
        })}
      >
        <div
          className={css({
            h: "full",
            pt: "4",
            pb: "6",
            lg: { pt: "4", pb: "8" },
          })}
        >
          <SidebarContent items={sidebarItems} />
        </div>
      </aside>
      {children}
    </div>
  );
};

export const Sidebar = ({
  children,
  ...props
}: {
  children: React.ReactNode;
  sidebarItems: SidebarItem[];
}) => {
  return (
    <DesktopSidebar {...props}>
      <MobileSidebar {...props}>{children}</MobileSidebar>
    </DesktopSidebar>
  );
};
