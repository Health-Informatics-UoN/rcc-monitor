import { options } from "@/auth/options";
import LandingPage from "@/components/home/LandingPage";
import UserHome from "@/components/home/UserHome";
import { getServerSession } from "next-auth";

export default async function Home() {
  const session = await getServerSession(options);

  return <>{session ? <UserHome /> : <LandingPage />}</>;
}
