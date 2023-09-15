import KeycloakProvider from "next-auth/providers/keycloak"
import { JWT } from "next-auth/jwt";
import { Account, Session } from "next-auth";

interface SessionWithToken extends Session {
  id_token?: string
} 

export const options  = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_ID || "",
      clientSecret: process.env.KEYCLOAK_SECRET || "",
      issuer: process.env.KEYCLOACK_ISSUER,

    })
  ],
  // Get the id_token and add it to the session
  callbacks: {
    async jwt({token, account} : { token: JWT, account?: Account}) {
      if (account) {
        // TODO: Decode the token and add roles.
        token = Object.assign({}, token, { id_token: account.id_token });
      }
      return token
    },

    async session({session, token} : { session: Session, token: JWT}) : Promise<SessionWithToken> {
      if(session) {
        session = Object.assign({}, session, { id_token: token.id_token })
      }
    return session
    }
  }
}
