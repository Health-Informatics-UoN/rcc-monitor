import { NextAuthOptions, Session } from "next-auth";
import { JWT } from "next-auth/jwt";
import { Provider } from "next-auth/providers/index";
import KeycloakProvider from "next-auth/providers/keycloak";

async function refreshAccessToken(token: JWT) {
  try {
    const url = process.env.KEYCLOAK_ISSUER + "/protocol/openid-connect/token";

    const params = new URLSearchParams({
      client_id: process.env.KEYCLOAK_ID || "",
      client_secret: process.env.KEYCLOAK_SECRET || "",
      grant_type: "refresh_token",
      refresh_token: token.refresh_token,
    });

    const response = await fetch(url, {
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      method: "POST",
      body: params,
    });

    const refreshedTokens = await response.json();

    if (!response.ok) {
      throw refreshedTokens;
    }

    return {
      ...token,
      access_token: refreshedTokens.access_token,
      expires_at: Date.now() / 1000 + refreshedTokens.expires_in,
      refresh_token: refreshedTokens.refresh_token ?? token.refreshToken, // Fall back to old refresh token
    };
  } catch (error) {
    console.error(error);

    return {
      ...token,
      error: "RefreshAccessTokenError",
    };
  }
}

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

      // Token has not expired yet so return it.
      if (now < token.expires_at) return token;

      // Token has expired so update and return it.
      return refreshAccessToken(token);
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
