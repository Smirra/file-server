export default function PrimaryLayout({
    children,
}: {
    children: React.ReactNode
}) {
    return (
        <div className='main-window pt-20'>
            {children}
        </div>
    )
}