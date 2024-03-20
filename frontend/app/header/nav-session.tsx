'use client'

import Link from "next/link";
import SignoutLink from "./signout-link";
import { useEffect, useState } from "react";
import { UserDropdown } from "./user-dropdown";

export default function NavSession({ session }) {
    const [isClient, setIsClient] = useState(false)
    
    useEffect(() => {
        setIsClient(true)
    }, [])

    const getUser = () => { return JSON.parse(localStorage.getItem('user')) }

    if (session) {
        return (
            <nav className='flex justify-end w-1/3'>
                <ul className='flex space-x-5'>
                    <li className='object-right text-right'>
                        {isClient && <UserDropdown user={getUser()} />}
                    </li>
                    <li className='object-right'>
                        <SignoutLink />
                    </li>
                </ul>
            </nav>
        )
    }
    else {
        return (
            <nav className='flex justify-end w-1/3'>
                <ul className='flex space-x-5'>
                    <li className='object-right'>
                        <Link href="/signin">Sign In</Link>
                    </li>
                </ul>
            </nav>
        )
    }
}