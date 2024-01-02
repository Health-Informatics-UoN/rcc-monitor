"use client";

import { signIn, signOut } from "next-auth/react";
import { css } from "@/styled-system/css";
import { Button } from "@/components/shadow-ui/Button";

export const LoginButton = () => {
  return (
    <Button variant={"ghost"} onClick={() => signIn("keycloak")}>
      Sign in
    </Button>
  );
};

export const LogoutButton = () => {
  return (
    <button onClick={() => signOut({ callbackUrl: "/" })} className={css({})}>
      Sign Out
    </button>
  );
};
