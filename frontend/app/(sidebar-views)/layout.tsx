import { sidebarLinks } from "../lib/links"
import Links from "../ui/links"

export default function SidebarLayout({
    children
}: {
    children: React.ReactNode
}) {
    return (
        <>
            <div className="absolute flex h-[calc(100vh-theme(spacing.header-h))] w-24 text-xl bg-white py-8 border-r-2 border-gray-400 shadow-md">
                <nav className="flex flex-col space-y-5 w-full pl-3">
                    <Links links={sidebarLinks} />
                </nav>
            </div>
            <div className='main-window pl-24 pt-32 overflow-auto'>
                {children}
            </div>
        </>
    )
}