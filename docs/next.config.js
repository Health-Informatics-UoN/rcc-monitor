/** @type {import('next').NextConfig} */
const nextConfig = {
  basePath: "/rcc-monitor",
  assetPrefix: "/rcc-monitor",
  output: "export",
  images: {
    unoptimized: true,
  },
};

const withNextra = require("nextra")({
  theme: "nextra-theme-docs",
  themeConfig: "./theme.config.jsx",
});

module.exports = withNextra(nextConfig);
