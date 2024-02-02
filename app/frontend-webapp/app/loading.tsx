import { css } from "@/styled-system/css";

/**
 * Base loading state for the entire app.
 */
export default function Loading() {
  return (
    <div
      className={css({
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        h: "screen",
      })}
    >
      <div
        className={css({
          transform: "rotate(360deg)",
          animation: "spin",
          rounded: "full",
          h: "32",
          w: "32",
          borderTopWidth: "2px",
          borderBottomWidth: "2px",
        })}
      ></div>
    </div>
  );
}
