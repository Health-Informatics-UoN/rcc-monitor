"use client";

import { SessionProvider, SessionProviderProps } from "next-auth/react";
import { ThemeProvider as NextThemesProvider } from "next-themes";
import * as React from "react";

export function Providers({ children, ...props }: SessionProviderProps) {
  return (
    <SessionProvider {...props}>
      <NextThemesProvider>{children}</NextThemesProvider>
    </SessionProvider>
  );
}
