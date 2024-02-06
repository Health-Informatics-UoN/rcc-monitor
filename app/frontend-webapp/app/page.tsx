import { getServerSession } from "next-auth";

import LandingPage from "@/components/home/LandingPage";
import UserHome from "@/components/home/UserHome";
import { options } from "@/lib/auth";

export default async function Home() {
  const session = await getServerSession(options);

  return <>{session ? <UserHome /> : <LandingPage />}</>;
}
