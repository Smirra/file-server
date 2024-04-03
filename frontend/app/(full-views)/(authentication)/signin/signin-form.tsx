'use client'

import { useRouter } from "next/navigation";
import { handleSignin } from "./actions"
import { obfuscate } from "@/app/lib/obfuscate";
import AlertMessage from "../../../ui/alert-message";
import { useState } from "react";

export default function SignInForm() {
    const router = useRouter();
    const [alertMsg, setAlertMsg] = useState({ title: '', body: '', show: false });
    
    const formAction = (async (formData: FormData) => {
        const obfuscatedPw = obfuscate(formData.get('password') as string);
        formData.set('password', obfuscatedPw);
        let res = await handleSignin(formData);
        
        if (res.user) {
            localStorage.setItem('user', JSON.stringify({ userName: res.user.userName, isAdmin: res.user.isAdmin }));

            router.push('/');
        }

        if (res.statusCode) {
            setAlertMsg({ title: 'Login failed', body: res.rspMsg , show: true });
        }
    })

    return (
        <div className="relative">
            <AlertMessage className="bottom-[21em]" message={alertMsg} />
            <form className="border-2 border-gray-400 p-10 shadow-md bg-white rounded-2xl"
                action={formAction} onChange={() => setAlertMsg({...alertMsg, show: false})}>
                <div className="mb-4">
                    <label className="block">Email:</label>
                    <input className="shadow appearance-none border rounded w-full py-2 px-3 
            text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                        type="email" name="email" placeholder="bob@bob.com" />
                </div>
                <div className="mb-10">
                    <label className="block">Password:</label>
                    <input className="shadow appearance-none border rounded w-full py-2 px-3 
            text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                        type="password" name="password" placeholder="***********" />
                </div>
                <div className="flex justify-center">
                    <button className="bg-gray-50 hover:bg-gray-100 border-gray-400 border-2 font-semibold py-3 px-4 rounded-2xl shadow-md transition"
                        type="submit">Sign in</button>
                </div>
            </form>
        </div>
    )
}