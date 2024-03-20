export default function PrimaryLayout({
    children,
}: {
    children: React.ReactNode
}) {
    return (
        <div className='h-full flex pt-20 justify-center bg-blue-100 bg-opacity-25 border-t-2 border-gray-400 shadow-inner-md'>
            {children}
        </div>
    )
}