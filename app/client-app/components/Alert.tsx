import "@/styles/alert.css";
import { AlertType } from "@/types/index";

export default function Alert({message}: AlertType) {
  return <div className="alert-bar">{message}</div>;
}
