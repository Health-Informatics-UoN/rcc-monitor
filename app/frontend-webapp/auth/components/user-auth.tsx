"use client";

import { signIn, signOut } from "next-auth/react";

import { Button } from "@/components/ui/button";

export const LoginButton = () => {
  return (
    <Button variant={"ghost"} onClick={() => signIn("keycloak")}>
      Sign in
    </Button>
  );
};

export const LogoutButton = () => {
  return (
    <button
      className="w-full text-left"
      onClick={() => signOut({ callbackUrl: "/" })}
    >
      Sign Out
    </button>
  );
};
