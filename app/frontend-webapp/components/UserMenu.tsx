import { LogOut, Settings } from "lucide-react";
import { getServerSession } from "next-auth";

import { LoginButton, LogoutButton } from "@/auth/components/user-auth";
import { options } from "@/auth/AuthOptions";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@/components/shadow-ui/Avatar";
import { Button } from "@/components/shadow-ui/Button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuShortcut,
  DropdownMenuTrigger,
} from "@/components/shadow-ui/DropdownMenu";
import { icon } from "@/styled-system/recipes";

export async function UserMenu() {
  const session = await getServerSession(options);

  if (!session) {
    return <LoginButton />;
  }

  const initials =
    session.user?.name
      ?.split(" ")
      .map((word) => word[0].toUpperCase())
      .join("") ?? "";

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost">
          {session.user?.name}
          <Avatar>
            <AvatarImage
              src={session.user?.image ?? ""}
              alt={session.user?.name ?? ""}
            />
            <AvatarFallback>{initials}</AvatarFallback>
          </Avatar>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent w="56">
        <DropdownMenuLabel>My Account</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {/* TODO: Add user settings here */}
        {/* <DropdownMenuGroup>
          <DropdownMenuItem>
            <Settings className={icon()} />
            <span>Settings</span>
            <DropdownMenuShortcut>⌘S</DropdownMenuShortcut>
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <DropdownMenuSeparator /> */}

        <DropdownMenuItem>
          <LogOut className={icon()} />
          <LogoutButton />
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
