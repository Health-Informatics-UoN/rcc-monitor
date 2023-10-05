import { defineConfig } from "@pandacss/dev";

export default defineConfig({
  // Required: Add the preset to config.
  presets: ["@shadow-panda/preset"],

  // Whether to use css reset
  preflight: true,

  // Where to look for your css declarations
  include: ["./components/**/*.{ts,tsx,js,jsx}", "./app/**/*.{ts,tsx,js,jsx}"],

  conditions: {
    light: ".light &",
    dark: "[data-theme=dark] &",
  },

  theme: {
    extend: {
      tokens: {
        colors: {
          primary: { value: "#38bdf8" },
        },
      },
      semanticTokens: {
        colors: {
          bg: {
            value: { base: "white", _dark: "black" },
          },
          foreground: {
            value: { base: "black", _dark: "white" },
          },
          card: {
            value: { base: "#fafafa", _dark: "#1e293b" },
          },
          cardForeground: {
            value: { base: "black", _dark: "white" },
          },
        },
      },
      textStyles: {
        h1: {
          value: {
            fontSize: "14.5rem",
            lineHeight: "1",
            letterSpacing: "tighter",
          },
        },
        h2: {
          value: {
            fontSize: { base: "2.5rem", lg: "3rem" },
            lineHeight: "1.2",
            letterSpacing: "tight",
          },
        },
        h3: {
          value: {
            fontSize: { base: "1.875rem", lg: "2.25rem" },
            lineHeight: "1.2",
            letterSpacing: "tight",
          },
        },
        h4: {
          value: {
            fontSize: "1.625rem",
            lineHeight: "1.2",
            letterSpacing: "tight",
          },
        },
      },
    },
  },

  // Files to exclude
  exclude: [],

  // Generate JSX Components for Patterns
  jsxFramework: "react",

  // The output directory for your css system
  emitPackage: false,
  outdir: "styled-system",

  staticCss: {
    recipes: {
      // Load toast variant styles since it cannot be statically analyzed
      toast: [{ variant: ["*"] }],
    },
  },
});
