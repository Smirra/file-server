import Link from "next/link";

export default function NavLinks({ session }) {
    return (
        <nav className="flex w-2/3">
            <ul className="flex space-x-5">
                <li>
                    <Link href="/">Home</Link>
                </li>
                <li>
                    <Link href="/public">Public Page</Link>
                </li>
                {session &&
                    <>
                        <li>
                            <Link href="/secure">Secure Page</Link>
                        </li>
                        <li>
                            <Link href="/files">Files</Link>
                        </li>
                    </>
                }
            </ul>
        </nav>
    )
}