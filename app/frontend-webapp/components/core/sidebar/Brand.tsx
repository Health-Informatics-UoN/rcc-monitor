import { Activity } from "lucide-react";

export const Brand = () => {
  return (
    <a
      href="/"
      className="flex pl-4 pr-4 text-cyan-600 items-center hover:text-cyan-800 transition duration-300 ease-in-out"
    >
      <Activity className="icon-lg mr-2" />
      <span className="text-xl leading-loose tracking-tight">
        <strong>RedCap</strong> Monitor
      </span>
    </a>
  );
};
