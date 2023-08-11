import "@/styles/navbar.css";
import Link from "next/link";
import { NavButtonType } from "@/types/navbar";

const NavButton = ({ children, to }: NavButtonType) => {
  return (
    <Link href={to}>
      <button className="custom-button">{children}</button>
    </Link>
  );
};

export default function Navbar() {
  return (
    <nav className="navbar">
      <div className="logo">NUH Collaboration</div>
      <div className="nav-menu">
        <NavButton to="/">Home</NavButton>
        <NavButton to="/reports">Reports</NavButton>
        <img
          src="https://www.clipartmax.com/png/middle/119-1198197_anonymous-person-svg-png-icon-free-download-anonymous-icon-png.png"
          alt="User Avatar"
          className="avatar"
        />
        <span className="user-name">User Name</span>
      </div>
    </nav>
  );
}
