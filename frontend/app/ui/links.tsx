'use client'

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Links } from "@/app/lib/links";

export default function Links({ links }: { links: Links }) {
    const pathname = usePathname();
    
    const basePath = pathname.split("/")[1];
    const currentLinks = links[basePath as keyof Links] || [];

    return (
        <>
            {currentLinks.map((link: { path: string; label: string; }) => (
                <div key={link.path}>
                    <Link className={pathname === link.path ? "font-medium" : ""}
                    href={link.path}>
                        {link.label}
                    </Link>
                </div>
            ))}
        </>
    );
}