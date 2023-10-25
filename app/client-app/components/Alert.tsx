import { css } from "@/styled-system/css";

interface AlertProps {
  css?: object;
  message: string;
}

export default function Alert({ css: cssProp = {}, message }: AlertProps) {
  return (
    <div
      className={css(
        {
          p: "10px",
          w: "50%",
          bgColor: "#f8d7da",
          color: "#721c24",
          textAlign: "center",
          fontWeight: "bold",
          border: "1px solid #f5c6cb",
          borderRadius: "4px",
        },
        cssProp
      )}
    >
      {message}
    </div>
  );
}
