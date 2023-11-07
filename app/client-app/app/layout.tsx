import { Container } from "@/styled-system/jsx";
import "@/styles/globals.css";

import Navbar from "@/components/Navbar";
import { Toaster } from "@/components/ui/toast/toaster";
import { Providers } from "@/app/providers";

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
          <Container>{children}</Container>
          <Toaster />
        </Providers>
      </body>
    </html>
  );
}
