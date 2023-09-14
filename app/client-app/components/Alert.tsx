import { css } from "@/styled-system/css";

export default function Alert({ message }: { message: string }) {
  return (
    <div
      className={css({
        p: "10px",
        w: "50%",
        m: "20px auto",
        bgColor: "#f8d7da",
        color: "#721c24",
        textAlign: "center",
        fontWeight: "bold",
        border: "1px solid #f5c6cb",
        borderRadius: "4px",
      })}
    >
      {message}
    </div>
  );
}
