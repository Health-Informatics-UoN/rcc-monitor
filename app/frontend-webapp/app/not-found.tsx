import Link from "next/link";

import { Button } from "@/components/ui/button";

export default function NotFound() {
  return (
    <div className="flex flex-col gap-4 items-start">
      <h1 className="h1">404 Not Found.</h1>
      <p>The requested page could not be found.</p>
      <Button asChild variant={"link"}>
        <Link href="/">Return Home</Link>
      </Button>
    </div>
  );
}
