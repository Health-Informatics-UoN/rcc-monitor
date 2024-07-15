import { MenuIcon } from "lucide-react";

import { UserMenu } from "@/components/core/UserMenu";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";

import { Brand } from "./Brand";
import { SidebarContent } from "./SidebarContent";
import { SidebarItem } from "./SidebarItem";

const MobileSidebar = ({
  children,
  sidebarItems,
}: {
  children: React.ReactNode;
  sidebarItems: SidebarItem[];
}) => {
  return (
    <>
      <Sheet>
        <div className="w-[90%] mt-5 mr-8 ml-6 z-10 flex items-center md:hidden">
          <div className="flex grow">
            <Brand />
          </div>
          <UserMenu />
          <SheetTrigger className="cursor-pointer">
            <MenuIcon size={24} />
          </SheetTrigger>
        </div>

        <SheetContent side="left">
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
    <div className="h-screen flex flex-col items-start overflow-x-hidden md:grid md:grid-cols-[200px_minmax(0,1fr)] md:gap-6 lg:grid lg:grid-cols-[260px_minmax(0,1fr)] lg:gap-10">
      <aside className="sticky top-0 z-30 hidden h-screen w-full shrink-0 border-r border-gray-200 md:sticky md:block">
        <div className="h-full pt-4 pb-6 lg:pt-4 lg:pb-8">
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
