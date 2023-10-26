/* eslint-disable @next/next/no-img-element */
import Link from "next/link";
import { LoginButton, LogoutButton } from "@/auth/components/user-auth";
import { getServerSession } from "next-auth";
import { options } from "@/auth/options";
import { permissions, hasPermission } from "@/auth/permissions";
import { css } from "@/styled-system/css";
import { flex } from "@/styled-system/patterns";
import { getServerConfig } from "@/lib/api/config";

interface NavButtonProps {
  css?: object;
  to: string;
  children: React.ReactNode;
}

const NavButton = ({ css: cssProp = {}, children, to }: NavButtonProps) => {
  return (
    <Link href={to}>
      <button
        className={css(
          {
            p: "15px",
            fontSize: "17px",
            fontWeight: "bold",
            _hover: { bg: "#50a7de" },
          },
          cssProp
        )}
      >
        {children}
      </button>
    </Link>
  );
};

export default async function Navbar() {
  const session = await getServerSession(options);
  const {
    siteMonitoringEnabled,
    syntheticDataEnabled,
    studyManagementEnabled,
  } = await getServerConfig();

  return (
    <nav
      className={flex({
        h: "60px",
        position: "relative",
        alignItems: "center",
        justifyContent: "space-between",
        bg: "linear-gradient(to right, #56a1d1, #0074d9)",
      })}
    >
      <div
        className={css({
          color: "white",
          fontSize: "1.5rem",
          fontWeight: "bold",
          ml: "25px",
        })}
      >
        <Link href="/">RedCap Monitor</Link>
      </div>
      <div
        className={flex({ color: "white", alignItems: "center", mr: "30px" })}
      >
        {hasPermission(session?.permissions, permissions.ViewStudies) &&
          studyManagementEnabled && (
            <NavButton to="/studies">Studies</NavButton>
          )}

        {hasPermission(session?.permissions, permissions.ViewSiteReports) &&
          siteMonitoringEnabled && <NavButton to="/reports">Reports</NavButton>}

        {hasPermission(
          session?.permissions,
          permissions.GenerateSyntheticData
        ) &&
          syntheticDataEnabled && (
            <NavButton to="/synthetic-data">Synthetic Data</NavButton>
          )}

        <div>{session ? <LogoutButton /> : <LoginButton />}</div>
      </div>
    </nav>
  );
}
