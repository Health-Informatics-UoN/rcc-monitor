import { css } from "@/styled-system/css";

export default function ContentPage() {
  return (
    <>
      <h2
        className={css({
          m: "10px 10px 0px 10px",
          p: "10px",
          fontSize: "35px",
          fontWeight: "500",
        })}
      >
        RedCap Monitoring
      </h2>
      <p
        className={css({
          m: "0px 10px",
          p: "10px",
          fontSize: "18px",
        })}
      >
        Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
        tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
        veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea
        commodo consequat. Duis aute irure dolor in reprehenderit in voluptate
        velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
        occaecat cupidatat non proident, sunt in culpa qui officia deserunt
        mollit anim id est laborum.
      </p>
    </>
  );
}
