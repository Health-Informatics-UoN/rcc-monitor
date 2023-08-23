import "@/styles/navbar.css";
import Link from "next/link";
import { NavButtonType } from "@/types/navbar";
import { LoginButton, RegisterButton, LogoutButton, ProfileButton  } from "@/auth/components/user-auth";
import { getServerSession } from "next-auth";
import { options } from "@/auth/options";

const NavButton = ({ children, to }: NavButtonType) => {
  return (
    <Link href={to}>
      <button className="custom-button">{children}</button>
    </Link>
  );
};

export default async function Navbar() {
  const session = await getServerSession(options)
  return (
    <nav className="navbar">
      <div className="logo">NUH Collaboration</div>
      <div className="nav-menu">
        <NavButton to="/">Home</NavButton>
        <NavButton to="/reports">Reports</NavButton>
        {JSON.stringify(session)}
        <div>
          <LoginButton />
          <RegisterButton />
          <LogoutButton />
          <ProfileButton />
        </div>
      </div>
    </nav>
  );
}
