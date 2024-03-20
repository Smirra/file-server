'use server'

import { cookies } from "next/headers";
import { parseCookie } from "@/app/lib/parse-cookie";

export async function handleRegistering(formData: FormData) {
    const body = Object.fromEntries(formData);
    const res = await fetch(`${process.env.INTERNAL_BASE_URL_IDENTITY}/user/register?` + new URLSearchParams({
        obfuscate: 'true'
    }), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({...body, rememberMe: false}),
        cache: 'no-store'
    })

    const jsonRsp = await res.json();

    if (!res.ok) {
        return { errors: { 'Errors':  jsonRsp} }
    }

    const sessionCookie = res.headers.getSetCookie().find(cookie => cookie.startsWith('session'));
    let parsedCookie = parseCookie(sessionCookie);

    if (!parsedCookie) {
        return { errors: { 'Cookie': "Invalid session" } } 
    }
    
    cookies().set(parsedCookie)

    return jsonRsp;
}