import Navbar from "@/components/Navbar";
import { ThemeProvider } from "@/components/theme-provider";
import { AuthProvider } from "@/auth/components/auth-provider";
import "@/styles/globals.css";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthProvider>
      <html lang="en" suppressHydrationWarning>
        <body>
          <ThemeProvider>
            <Navbar />
            {children}
          </ThemeProvider>
        </body>
      </html>
    </AuthProvider>
  );
}
