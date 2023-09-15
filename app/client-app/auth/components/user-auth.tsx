"use client";

import { signIn, signOut } from "next-auth/react";

export const LoginButton = () => {
  return (
    <button onClick={() => signIn('keycloak')} className="custom-button">
      Sign in
    </button>
  );
};

export const LogoutButton = () => {
  return (
    <button onClick={() => signOut()} className="custom-button">
      Sign Out
    </button>
  );
};
