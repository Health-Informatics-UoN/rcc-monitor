/* eslint-disable @typescript-eslint/no-unused-vars */
import NextAuth from "next-auth";
import { Session } from "next-auth";
import { KeycloakProfile } from "next-auth/providers/keycloak";

import { Permission } from "@/lib/auth";

declare module "next-auth" {
  /**
   * Returned by `useSession`, `getSession` and received as a prop on the `SessionProvider` React Context
   */
  interface Session extends Session {
    token: JWT;
    access_token: string;
  }
  interface Profile extends KeycloakProfile {}
}

declare module "next-auth/jwt" {
  /** Returned by the `jwt` callback and `getToken`, when using JWT sessions */
  interface JWT {
    id_token: string;
    access_token: string;
    permissions: Permission[];
    expires_at: number;
    refresh_token: string;
  }
}
