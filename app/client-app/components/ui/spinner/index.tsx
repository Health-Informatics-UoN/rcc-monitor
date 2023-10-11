import { css } from "@/styled-system/css";
import { Flex } from "@/styled-system/jsx";

export default function Spinner() {
  return (
    <Flex justifyContent="center" alignItems="center" h="100%" m="auto 5px">
      <div
        className={css({
          border: " 3px solid rgba(0, 0, 0, 0.3)",
          borderRadius: "50%",
          borderTop: "3px solid black",
          width: "30px",
          height: "30px",
          animation: "spin",
        })}
      ></div>
    </Flex>
  );
}
