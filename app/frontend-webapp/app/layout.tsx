import "./globals.css";

import { Metadata } from "next";

import { Providers } from "@/app/providers";
import Navbar from "@/components/core/Navbar";
import { Sidebar } from "@/components/core/sidebar/Sidebar";
import { Toaster } from "@/components/ui/toaster";
import { SidebarItems } from "@/config/sidebar";

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
            <div className="relative max-w-8xl mx-auto px-4 md:px-6 lg:px-8 w-full mt-20">
              {children}
            </div>
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
