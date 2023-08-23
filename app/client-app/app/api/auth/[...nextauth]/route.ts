import NextAuth from "next-auth"
import KeycloakProvider from "next-auth/providers/keycloak"

const options  = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_ID || "",
      clientSecret: process.env.KEYCLOAK_SECRET || "",
      issuer: process.env.KEYCLOACK_ISSUER,

    })
  ]
}

const handler = NextAuth(options)

export { handler as GET, handler as POST }
