import KeycloakProvider from "next-auth/providers/keycloak"

export const options  = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_ID || "",
      clientSecret: process.env.KEYCLOAK_SECRET || "",
      issuer: process.env.KEYCLOACK_ISSUER,

    })
  ],
  callbacks: {
    async jwt({token, account}) {
      if (account) {
        token = Object.assign({}, token, { access_token: account.id_token });
      }
      return token
    },
    async session({session, token}) {
    if(session) {
      session = Object.assign({}, session, {access_token: token.access_token})
      console.log(session);
      }
    return session
    }
  }
}
