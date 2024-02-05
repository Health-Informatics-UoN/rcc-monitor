"use client";

import { Field, FieldArray } from "formik";
import { Search } from "lucide-react";
import { useParams } from "next/navigation";
import { useSession } from "next-auth/react";
import * as React from "react";
import { useDebouncedCallback } from "use-debounce";

import { getUnaffiliated } from "@/api/users";
import { Permissions } from "@/auth/Permissions";
import { Button } from "@/components/shadow-ui/Button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "@/components/shadow-ui/Command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/shadow-ui/Popover";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/shadow-ui/Table";
import { isUserAuthorized } from "@/lib/auth/isUserAuthorized";
import { css } from "@/styled-system/css";
import { Grid } from "@/styled-system/jsx";
import { User } from "@/types/users";

export const UserManagement = ({ users }: { users: User[] }) => {
  const { data: session } = useSession();

  return (
    <Grid gridTemplateColumns="1" alignItems="center" gap="4">
      <FieldArray name="users">
        {({ push, remove }) => (
          <>
            <UserSearch push={push} />

            <Table gridColumn={"3"} margin={"auto"}>
              <TableHeader>
                <TableRow>
                  <TableHead>Name</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead></TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map((user, index) => (
                  <TableRow key={index}>
                    <Field
                      name={`studyUsers[${index}].name`}
                      value={user.name}
                      component={TableCell}
                    >
                      {user.name}
                    </Field>
                    <Field
                      name={`studyUsers[${index}].email`}
                      value={user.email}
                      component={TableCell}
                    >
                      {user.email}
                    </Field>
                    {isUserAuthorized(
                      session?.permissions,
                      Permissions.RemoveStudyUsers
                    ) && (
                      <TableCell>
                        <Button
                          type="button"
                          onClick={() => {
                            remove(index);
                          }}
                          variant={"destructive"}
                        >
                          Remove
                        </Button>
                      </TableCell>
                    )}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </>
        )}
      </FieldArray>
    </Grid>
  );
};

function UserSearch({ push }: { push: (user: User) => void }) {
  const params = useParams();

  const [open, setOpen] = React.useState<boolean>(false);
  const [loading, setLoading] = React.useState<boolean>(false);
  const [items, setItems] = React.useState<User[]>();

  /**
   * Get users from the backend to list.
   * @param query user to search for.
   */
  const handleQuery = useDebouncedCallback(async (query: string) => {
    setLoading(true);
    if (query.length > 2) {
      const data = await getUnaffiliated(Number(params.id), query);
      setItems(data);
    }
    setLoading(false);
  }, 300);

  /**
   * Handle adding the selected user.
   * @param value email selected
   */
  function handleAddUser(value: string) {
    // Get the user from the list
    const user = items?.find(({ email }) => email === value);
    if (user) {
      push(user);
    }
  }

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className={css({
            justifyContent: "between",
            gridColumn: "3",
          })}
        >
          Search users
          <Search
            className={css({
              ml: "2",
              h: "4",
              w: "4",
              flexShrink: 0,
              opacity: "0.5",
            })}
          />
        </Button>
      </PopoverTrigger>
      <PopoverContent
        className={css({
          w: "400px",
          p: "0",
        })}
      >
        <Command>
          <CommandInput
            placeholder="Search users..."
            onValueChange={handleQuery}
          />
          <CommandEmpty>No users found.</CommandEmpty>
          <CommandGroup>
            {loading && <CommandItem>Searching...</CommandItem>}
            {items?.map((user) => (
              <CommandItem
                key={user.id}
                onSelect={(value) => {
                  handleAddUser(value);
                  setOpen(false);
                }}
                value={user.email}
              >
                {user.name} - {user.email}
              </CommandItem>
            ))}
          </CommandGroup>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
