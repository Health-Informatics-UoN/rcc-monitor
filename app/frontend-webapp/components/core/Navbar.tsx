import React from "react";

import { UserMenu } from "@/components/core/UserMenu";

export const Navbar = () => {
  return (
    <nav className="hidden md:flex items-center w-full fixed pl-4 pr-4 h-16">
      <div className="grow"></div>
      <UserMenu />
    </nav>
  );
};
export default Navbar;
