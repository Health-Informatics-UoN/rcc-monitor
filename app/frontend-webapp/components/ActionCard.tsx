import { css } from "@/styled-system/css";
import { Box } from "@/styled-system/jsx";
import Link from "next/link";

interface ActionCardProps {
  to: string;
  title: string;
  icon?: React.ReactNode;
  description: string;
}

export default function ActionCard({
  to,
  title,
  icon,
  description,
}: ActionCardProps) {
  return (
    <Link href={to}>
      <Box
        bg="bg"
        borderColor="gray.300"
        borderWidth={2}
        borderRadius={5}
        h="100%"
        w={["450px", "350px", "450px"]}
        p={4}
        _hover={{
          borderColor: "blue.500",
          color: "blue.500",
          bg: "gray.50",
        }}
      >
        <div className={css({ display: icon ? "flex" : "", mb: "5" })}>
          <Box m="auto 10px">{icon && icon}</Box>
          <h1 className={css({ fontSize: "xl", fontWeight: "medium" })}>
            {title}
          </h1>
        </div>
        <p className={css({ mb: "8px" })}>{description}</p>
      </Box>
    </Link>
  );
}
