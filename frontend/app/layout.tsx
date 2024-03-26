import './globals.css'
import Header from '@/app/header/header'

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {

  return (
    <html lang="en">
      <body>
        <Header />
        <div className="w-[100vw] h-[calc(100dvh-theme('spacing.header-h'))]">
          {children}
        </div>
      </body>
    </html>
  )
}