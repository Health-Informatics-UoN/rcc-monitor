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

## NextAuth Config

You will need to setup the `next-auth` [API endpoint](https://next-auth.js.org/getting-started/example#add-api-route) in your app:

```typescript
// app/api/auth/[...nextauth]/route.ts
import NextAuth from "next-auth";

import { options } from "@/lib/auth";

const handler = NextAuth(options);

export { handler as GET, handler as POST };
```

And implement it's [session provider](https://next-auth.js.org/getting-started/example#configure-shared-session-state) if you want to use the client side authorization at all, import the providers into your `layout.tsx`

```typescript
// app/providers.tsx
"use client";

import * as React from "react";
import { SessionProvider, SessionProviderProps } from "next-auth/react";

export function Providers({ children, ...props }: SessionProviderProps) {
  return (
    <SessionProvider {...props}>
      {children}
    </SessionProvider>
  );
}
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

## Define your Route Mapping

Define which routes need a policy requirement, and map them to your Policies.

```typescript
// auth/RouteAuthMapping.ts
import { RouteAuthorizationMapping } from "@/lib/auth";
import { AuthorizationPolicies } from "@/auth/AuthPolicies";

// Map the route and its required policy that needs to be authenticated.
export const routeAuthMapping: RouteAuthorizationMapping = {
  "/reports": AuthorizationPolicies.CanViewReports,
  // You can map dynamic routes as well: 
  "/reports/:id": AuthorizationPolicies.CanViewReports 
};
```

## Implement the Middleware

To use the middleware, in your root `middleware.ts` (create one if you don't have it already), pass your route mapping to our `isAuthorized` middleware, which is a reponse to the NextAuth `authorized` callback.

You will need a config matcher to define which paths the middleware should run on as well, see the [Middleware documentation](https://nextjs.org/docs/app/building-your-application/routing/middleware) for details.

```typescript
// middleware.ts
import { withAuth } from "next-auth/middleware";
import { isAuthorized } from "@/lib/auth";
import { routeAuthMapping } from "@/auth/routeAuthMapping";

export default withAuth({
  callbacks: {
    authorized: getAuthorized(routeAuthMapping),
  },
});

// We can't set a default config in /lib/auth until it supports dynamic config
// https://nextjs.org/docs/messages/invalid-page-config
// Here's an example of how it might look:
export const config = {
  matcher: [
    /*
     * Match all paths except for:
     * 1. / index page
     * 2. /api routes
     * 3. /_next (Next.js internals)
     * 4. /_static (inside /public)
     * 5. all root files inside /public (e.g. /favicon.ico)
     */
    "/((?!$|api/|_next/|_static/|[\\w-]+\\.\\w+).*)",
  ],
};
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
