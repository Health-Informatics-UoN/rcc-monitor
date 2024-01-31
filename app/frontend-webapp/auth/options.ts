import { NextAuthOptions, Session } from "next-auth";
import { JWT } from "next-auth/jwt";
import { Provider } from "next-auth/providers/index";
import KeycloakProvider from "next-auth/providers/keycloak";

export const options: NextAuthOptions = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_ID || "",
      clientSecret: process.env.KEYCLOAK_SECRET || "",
      issuer: process.env.KEYCLOAK_ISSUER,
    }),
  ] as Provider[],
  secret: process.env.NEXTAUTH_SECRET,

  // Get the id_token and roles and add to the session.
  callbacks: {
    async jwt({ token, account, profile }) {
      const now = Math.floor(Date.now() / 1000);

      if (account && profile) {
        // runs on sign in only.
        token = Object.assign({}, token, {
          id_token: account.id_token,
          access_token: account.access_token,
          permissions: profile.realm_access?.roles,
          expires_at: account.expires_at,
          refresh_token: account.refresh_token,
        });
      }

      // token has not expired yet so return it.
      if (now < token.expires_at) return token;

      // TODO: Token has expired to update it
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
          access_token: token.access_token,
          permissions: token.permissions,
        });
      }
      return session;
    },
  },
};
