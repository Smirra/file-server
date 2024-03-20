import NavSession from "./nav-session";
import NavLinks from "./nav-links";
import { cookies } from "next/headers";

export default function Header() {
  const session = cookies().has('session')

  return (
    <header className="h-header-h flex items-center justify-between text-xl px-10">
      <NavLinks session={session} />
      <NavSession session={session} />
    </header>
  )
}
