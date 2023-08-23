import KeycloakProvider from "next-auth/providers/keycloak"

export const options  = {
  providers: [
    KeycloakProvider({
      clientId: process.env.KEYCLOAK_ID || "",
      clientSecret: process.env.KEYCLOAK_SECRET || "",
      issuer: process.env.KEYCLOACK_ISSUER,

    })
  ]
}
