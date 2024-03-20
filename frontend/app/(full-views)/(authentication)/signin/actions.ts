'use server'

import { cookies } from "next/headers";
import { parseCookie } from "@/app/lib/parse-cookie";

export async function handleSignin(formData: FormData) {
    const body = Object.fromEntries(formData);

    const response = await fetch(`${process.env.INTERNAL_BASE_URL_IDENTITY}/user/signin?` + new URLSearchParams({
        obfuscate: 'true'
    }), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({...body, rememberMe: false}),
        cache: 'no-store'
    })

    const jsonRsp = await response.json();
    if (!response.ok) {
        return { errors: { 'Error': jsonRsp } }
    }

    const sessionCookie = response.headers.getSetCookie().find(cookie => cookie.startsWith('session'));
    let parsedCookie = parseCookie(sessionCookie);

    if (!parsedCookie) {
        return { errors: { 'Cookie': "Invalid session" } } 
    }
    
    cookies().set(parsedCookie)

    return jsonRsp 
}