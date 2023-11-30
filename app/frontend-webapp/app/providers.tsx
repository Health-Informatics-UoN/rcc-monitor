"use client";

import * as React from "react";
import { ThemeProvider as NextThemesProvider } from "next-themes";
import { SessionProvider, SessionProviderProps } from "next-auth/react";

export function Providers({ children, ...props }: SessionProviderProps) {
  return (
    <SessionProvider {...props}>
      <NextThemesProvider>{children}</NextThemesProvider>
    </SessionProvider>
  );
}
