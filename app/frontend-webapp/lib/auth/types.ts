import { JWT } from "next-auth/jwt";

export type Permission = string;

export type AuthorizationPolicy = {
  isAuthorized: (token: JWT | null) => boolean;
  getPermissions: () => Permission[];
};

export interface PathAuthorizationMapping {
  [path: string]: AuthorizationPolicy;
}
