import { redirect } from "next/navigation";
import { cookies, headers } from "next/headers"
import { NextResponse } from "next/server"

export const dynamic = 'force-dynamic'

export async function GET() {
    let res = await fetch(`${process.env.INTERNAL_BASE_URL_IDENTITY}/user/signout`, {
        method: 'POST',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        cache: 'no-store'
    })

    if (!res.ok) {
        return NextResponse.json({ error:  'Failed to logout.'  })
    }
    
    const headersList = headers();
    const referer = headersList.get('referer')

    cookies().delete('session')

    redirect(referer || '/')
}