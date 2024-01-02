import { Activity } from "lucide-react";

import { css } from "@/styled-system/css";
import { icon } from "@/styled-system/recipes";

export const Brand = () => {
  return (
    <a
      href="/"
      className={css({
        display: "flex",
        pl: "4",
        pr: "4",
        color: "cyan.600",
        alignItems: "center",
        transition: "color 0.3s ease",
        _hover: {
          color: "cyan.800",
        },
      })}
    >
      <Activity className={icon({ right: "sm", size: "lg" })} />
      <span
        className={css({
          fontSize: "xl",
          lineHeight: "2xl",
          letterSpacing: "tighter",
        })}
      >
        <strong>RedCap</strong> Monitor
      </span>
    </a>
  );
};
