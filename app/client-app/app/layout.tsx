import Navbar from "@/components/Navbar";
import { Toaster } from "@/components/ui/toast/toaster";
import "@/styles/globals.css";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>
        <Navbar />
        {children}
        <Toaster />
      </body>
    </html>
  );
}
