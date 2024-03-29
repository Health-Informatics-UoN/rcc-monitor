import React from "react";

import { UserMenu } from "@/components/core/UserMenu";
import { css } from "@/styled-system/css";

export const Navbar = () => {
  return (
    <nav
      className={css({
        hideBelow: "sm",
        display: "flex",
        alignItems: "center",
        w: "full",
        pos: "fixed",
        pl: "4",
        pr: "4",
        h: "16",
      })}
    >
      <div className={css({ flexGrow: "1" })}></div>
      <UserMenu />
    </nav>
  );
};
export default Navbar;
