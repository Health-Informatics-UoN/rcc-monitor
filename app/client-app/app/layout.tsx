import { Container } from "@/styled-system/jsx";
import "@/styles/globals.css";

import Navbar from "@/components/Navbar";
import { Toaster } from "@/components/ui/toast/toaster";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>
        <Navbar />
        <Container>{children}</Container>
        <Toaster />
      </body>
    </html>
  );
}
