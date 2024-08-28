// https://nextra.site/docs/docs-theme/theme-configuration
import { Activity } from "lucide-react";

export default {
  logo: (
    <span
      style={{
        display: "flex",
        color: "#0E7490",
      }}
    >
      <Activity style={{ marginRight: "5px" }} />
      <strong style={{ marginRight: "5px" }}>RedCap</strong> Monitor
      Documentation
    </span>
  ),
  project: {
    link: "https://github.com/Health-Informatics-UoN/rcc-monitor",
  },
  head: (
    <>
      <link rel="icon" type="image/svg+xml" href="./activity.png" />
    </>
  ),
  footer: {
    text: (
      <span>
        ©{new Date().getFullYear()}{" "}
        <a href="https://nottingham.ac.uk" target="_blank">
          University of Nottingham
        </a>
      </span>
    ),
  },
};
