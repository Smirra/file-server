import { NextRequest, NextResponse } from 'next/server'

export function middleware(request: NextRequest) {
    const session = request.cookies.get('session');

    const securePaths = /^\/(secure|files)(\/|$)/;
    const inSessionRedirects = /^\/(signin|signup)(\/|$)/;

    if (!session && securePaths.test(request.nextUrl.pathname)) {
        return NextResponse.rewrite(`${process.env.INTERNAL_BASE_URL}/signin`);
    }
    if (session && inSessionRedirects.test(request.nextUrl.pathname)) {
        return NextResponse.rewrite(process.env.INTERNAL_BASE_URL);
    }

    return NextResponse.next();
}