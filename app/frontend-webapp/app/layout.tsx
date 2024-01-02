import { Container } from "@/styled-system/jsx";
import "@/styles/globals.css";

import { Toaster } from "@/components/shadow-ui/Toast/Toaster";
import { Providers } from "@/app/providers";
import { css } from "@/styled-system/css";
import { Metadata } from "next";
import { Sidebar } from "@/components/sidebar/Sidebar";
import { SidebarItems } from "@/config/sidebar";
import Navbar from "@/components/Navbar";

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
              className={css({
                mt: 20,
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
