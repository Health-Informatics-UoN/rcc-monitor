import "@/styles/globals.css";

import { Metadata } from "next";

import { Providers } from "@/app/providers";
import Navbar from "@/components/Navbar";
import { Toaster } from "@/components/shadow-ui/Toast/Toaster";
import { Sidebar } from "@/components/sidebar/Sidebar";
import { SidebarItems } from "@/config/sidebar";
import { css } from "@/styled-system/css";
import { Container } from "@/styled-system/jsx";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>
        <Providers>
          <Navbar />
          <Sidebar sidebarItems={SidebarItems}>
            <Container
              w="100%"
              className={css({
                mt: 20,
                mb: 40,
              })}
            >
              {children}
            </Container>
          </Sidebar>
          <Toaster />
        </Providers>
      </body>
    </html>
  );
}

export const metadata: Metadata = {
  title: "RedCap Monitor",
  description: "A tool to monitor and enhance NUH RedCap Clinical Trials.",
  icons: {
    icon: "/icons/favicon.ico",
    apple: "/icons/apple-touch-icon.png",
  },
  manifest: "/manifest.json",
};
