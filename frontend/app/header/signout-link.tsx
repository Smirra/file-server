'use client'

import { useRouter } from 'next/navigation';
import React from 'react';

export default function SignoutLink() {
    const router = useRouter()

    const fetchRoute = async (e: any) => {
        e.preventDefault();

        const res = await fetch('/signout', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
            cache: 'no-store'
        })

        if (!res.ok) {
            throw new Error(`HTTP error! Status: ${res.status}`);
        }

        if (localStorage.getItem('username')) {
            localStorage.removeItem('username');
        }

        router.refresh() 
    }

    return (
        <a href='/signout'
            onClick={fetchRoute}
            >Sign Out</a>
    );
}