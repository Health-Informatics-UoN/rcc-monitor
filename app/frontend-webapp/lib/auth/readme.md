# NextAuth Keycloak PBAC Library

This code implements Policy Based Access Control (PBAC) for a Next.js application, using Keycloak for managing users and their roles.  

It enables you to define Authorization Policies similar to a .NET backend, and apply them to routing and rendering.

You should still authorize everything in your backend, and not trust this client application [as recommended](https://nextjs.org/blog/security-nextjs-server-components-actions).

## Dependencies

- `next-auth`
- `path-to-regexp`

You will also need a Keycloak instance with your realm and roles configured.

## Keycloak Configuration

The library defines the Keycloak provider for you, so all you need to do is provide the environment variables:

```bash
KEYCLOAK_ID
KEYCLOAK_SECRET
KEYCLOAK_ISSUER
NEXTAUTH_SECRET
```

## Define your Permissions

Define your permissions constants that will be passed from Keycloak in the token.

```typescript
// auth/Permissions.ts
import { assertPermission } from "@/lib/auth";

export class Permissions {
  public static readonly ViewReports = assertPermission("ViewReports");
}
```

## Define your Policies

Like your backend, build your policies using `AuthorizationPolicyBuilder` and your Permissions.

```typescript
// auth/AuthPolicies.ts
import { AuthorizationPolicyBuilder } from "@/lib/auth";
import { Permissions } from "@/auth/Permissions";

export class AuthorizationPolicies {

  public static readonly CanViewReports = new AuthorizationPolicyBuilder()
    .RequirePermissions(Permissions.ViewReports)
    .Build();
};
```

## Define your Path Mapping

Define which paths need a policy requirement, and map them to your Policies.

```typescript
// auth/PathAuthMapping.ts
import { PathAuthorizationMapping } from "@/lib/auth";
import { AuthorizationPolicies } from "@/auth/AuthPolicies";

// Map the path and its required policy that needs to be authenticated.
export const pathAuthMapping: PathAuthorizationMapping = {
  "/reports": AuthorizationPolicies.CanViewReports,
  // You can map dynamic paths as well: 
  "/reports/:id": AuthorizationPolicies.CanViewReports 
};
```

## Implement the Middleware

To use the middleware, in your root `middleware.ts` (create one if you don't have it already), pass your path mapping to our `isAuthorized` middleware, which is a reponse to the NextAuth `authorized` callback.

You will need a config matcher to define which paths the middleware should run on as well, see the [Middleware documentation](https://nextjs.org/docs/app/building-your-application/routing/middleware) for details.

```typescript
// middleware.ts
import { withAuth } from "next-auth/middleware";
import { isAuthorized } from "@/lib/auth";
import { pathAuthMapping } from "@/auth/PathAuthMapping";

export default withAuth({
  callbacks: {
    authorized: ({
      req,
      token,
    }) => {
      return isAuthorized({
        req,
        token,
        pathAuthMapping,
      });
    },
  },
});

// Matches on all paths.
export const config = { matcher: ['/'] }
```

## Rendering

To render content based on the users permissions, use the `isUserAuthorized` helper function.

On the server use `next-auth` `getServerSession()`, and on the client side use: `useSession()`

```typescript
// Home.tsx
import { getServerSession } from "next-auth";
import { isUserAuthorized, options } from "@/lib/auth";

export default async function Home() {
  const session = await getServerSession(options)
  const authed = isUserAuthorized(session?.token, AuthorizationPolicies.CanViewReports) 

  return(<>
      {authed && <Reports />}
  </>)
}
```
