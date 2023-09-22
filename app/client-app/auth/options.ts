import KeycloakProvider from "next-auth/providers/keycloak";
import { JWT } from "next-auth/jwt";
import { NextAuthOptions, Session } from "next-auth";
import { Provider } from "next-auth/providers/index";

export const options: NextAuthOptions = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_ID || "",
      clientSecret: process.env.KEYCLOAK_SECRET || "",
      issuer: process.env.KEYCLOACK_ISSUER,
    }),
  ] as Provider[],

  // Get the id_token and roles and add to the session.
  callbacks: {
    async jwt({ token, account, profile }) {
      if (account && profile) {
        token = Object.assign({}, token, {
          id_token: account.id_token,
          permissions: profile.realm_access?.roles,
        });
      }
      return token;
    },

    async session({
      session,
      token,
    }: {
      session: Session;
      token: JWT;
    }): Promise<Session> {
      if (session) {
        session = Object.assign({}, session, {
          id_token: token.id_token,
          permissions: token.permissions,
        });
      }
      return session;
    },
  },
};
