import { options } from "@/auth/options";
import ContentPage from "@/components/home/ContentPage";
import UserHome from "@/components/home/UserHome";
import { getServerSession } from "next-auth";

export default async function Home() {
  const session = await getServerSession(options);

  return <>{session ? <UserHome /> : <ContentPage />}</>;
}
