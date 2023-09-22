"use client";

import { signIn, signOut } from "next-auth/react";
import { css } from "@/styled-system/css";

export const LoginButton = () => {
  return (
    <button
      onClick={() => signIn("keycloak")}
      className={css({
        p: "15px",
        fontSize: "17px",
        fontWeight: "bold",
        _hover: { bg: "#50a7de" },
      })}
    >
      Sign in
    </button>
  );
};

export const LogoutButton = () => {
  return (
    <button
      onClick={() => signOut()}
      className={css({
        p: "15px",
        fontSize: "17px",
        fontWeight: "bold",
        _hover: { bg: "#50a7de" },
      })}
    >
      Sign Out
    </button>
  );
};
