import { css } from "@/styled-system/css";

export default function LandingPage() {
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
        RedCap Monitor
      </h2>
      <p
        className={css({
          m: "0px 10px",
          p: "10px",
          fontSize: "18px",
          lineHeight: "50px",
        })}
      >
        A tool to monitor and enhance NUH RedCap Clinical Trials. <br />
        Part of the University of Nottingham Digital Research Service and
        Nottingham University Hospital NHS Trust collaboration.
      </p>
    </>
  );
}
