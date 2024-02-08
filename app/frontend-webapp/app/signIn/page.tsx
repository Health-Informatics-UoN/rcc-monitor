"use client";

import { useRouter } from "next/navigation";
import { signIn, useSession } from "next-auth/react";
import { useEffect } from "react";

/**
 * A page purely to navigate a user directly to Keycloak should NextAuth ask to log in.
 * For use until NextAuth can use a default provider.
 * https://github.com/nextauthjs/next-auth/discussions/4800
 */
export default function Signin() {
  const router = useRouter();
  const { status } = useSession();

  useEffect(() => {
    if (status === "unauthenticated") {
      void signIn("keycloak");
    } else if (status === "authenticated") {
      void router.push("/");
    }
  }, [status, router]);

  return <div></div>;
}
