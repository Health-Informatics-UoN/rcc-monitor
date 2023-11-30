import { Container } from "@/styled-system/jsx";
import "@/styles/globals.css";

import Navbar from "@/components/Navbar";
import { Toaster } from "@/components/shadow-ui/Toast/Toaster";
import { Providers } from "@/app/providers";
import { css } from "@/styled-system/css";

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
          <Container
            className={css({
              mt: 6,
            })}
          >
            {children}
          </Container>
          <Toaster />
        </Providers>
      </body>
    </html>
  );
}
