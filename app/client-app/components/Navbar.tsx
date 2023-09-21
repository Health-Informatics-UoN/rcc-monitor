/* eslint-disable @next/next/no-img-element */
import Link from "next/link";
import { css } from "@/styled-system/css";
import { flex } from "@/styled-system/patterns";

interface NavButtonProps {
  css?: {};
  to: string;
  children: React.ReactNode;
}

const NavButton = ({ css: cssProp = {}, children, to }: NavButtonProps) => {
  return (
    <Link href={to}>
      <button
        className={css(
          {
            p: "15px",
            fontSize: "17px",
            fontWeight: "bold",
            _hover: { bg: "#50a7de" }
          },
          cssProp
        )}
      >
        {children}
      </button>
    </Link>
  );
};

export default function Navbar() {
  return (
    <nav
      className={flex({
        h: "60px",
        position: "relative",
        alignItems: "center",
        justifyContent: "space-between",
        bg: "linear-gradient(to right, #56a1d1, #0074d9)"
      })}
    >
      <div
        className={css({
          color: "white",
          fontSize: "1.5rem",
          fontWeight: "bold",
          ml: "25px"
        })}
      >
        RedCap Site Reports
      </div>
      <div
        className={flex({ color: "white", alignItems: "center", mr: "30px" })}
      >
        <NavButton to="/">Home</NavButton>
        <NavButton to="/reports" css={{ mr: "5px" }}>
          Reports
        </NavButton>
        <img
          src="https://www.clipartmax.com/png/middle/119-1198197_anonymous-person-svg-png-icon-free-download-anonymous-icon-png.png"
          alt="User Avatar"
          className={css({
            w: "40px",
            h: "40px",
            mr: "0.5rem",
            borderRadius: "50%"
          })}
        />
        <span className={css({ fontWeight: "500" })}>User Name</span>
      </div>
    </nav>
  );
}
