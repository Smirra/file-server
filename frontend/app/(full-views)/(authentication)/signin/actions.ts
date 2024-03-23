'use server'

import { cookies } from "next/headers";
import { parseCookie } from "@/app/lib/parse-cookie";

export async function handleSignin(formData: FormData) {
    const body = Object.fromEntries(formData);

    const rsp = await fetch(`${process.env.INTERNAL_BASE_URL_IDENTITY}/user/signin?` + new URLSearchParams({
        obfuscate: 'true'
    }), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ ...body, rememberMe: false }),
        cache: 'no-store'
    })

    if (rsp.status == 401 || rsp.status == 403) {
        let rspMsg = await rsp.text()
        return { statusCode: rsp.status, rspMsg }
    }

    if (rsp.ok) {
        const sessionCookie = rsp.headers.getSetCookie().find(cookie => cookie.startsWith('session'))
        let parsedCookie = parseCookie(sessionCookie)

        if (!parsedCookie) {
            return { errors: { 'Cookie': "Invalid session" } }
        }

        cookies().set(parsedCookie)

        return await rsp.json()
    }

    throw new Error(`HTTP error! Status: ${rsp.status}`)
}