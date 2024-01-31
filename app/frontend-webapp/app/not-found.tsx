import Link from "next/link";

import { Button } from "@/components/shadow-ui/Button";
import { vstack } from "@/styled-system/patterns";
import { h1 } from "@/styled-system/recipes";

export default function NotFound() {
  return (
    <div
      className={vstack({
        gap: "4",
        direction: "column",
        alignItems: "flex-start",
      })}
    >
      <h1 className={h1()}>404 Not Found.</h1>
      <p>The requested page could not be found.</p>
      <Button asChild variant={"link"}>
        <Link href="/">Return Home</Link>
      </Button>
    </div>
  );
}
