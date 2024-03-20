'use client'

import { handleRegistering } from "./actions";
import { useState, useTransition } from "react";
import InputErrorMessage from "../input-error-msg";
import { useRouter } from "next/navigation";
import { obfuscate } from "@/app/lib/obfuscate";

export function SignUpForm() {
    const [isPending, startTransition] = useTransition();
    const router = useRouter();

    const registerModel = {
        firstName: '',
        lastName: '',
        email: '',
        userName: '',
        password: '',
        confirmPassword: '',
    };

    const [input, setInput] = useState({ ...registerModel });
    const [error, setError] = useState({ ...registerModel });

    const onInputChange = (e: any) => {
        const { name, value } = e.target;

        setInput(prev => ({
            ...prev,
            [name]: value,
        }));

        validateInput(e);
    }

    const validateInput = (e: any) => {
        const { name, value } = e.target;
        let setCustomValidityMessage = '';

        setError(prev => {
            const errState = { ...prev };
            if (value) {
                switch (name) {
                    case 'firstName':
                        if (value.length < 2) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'First name must be at least 2 characters long';
                        }
                        break;

                    case 'lastName':
                        if (value.length < 2) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'Last name must be at least 2 characters long';
                        }
                        break;

                    case 'email':
                        errState[name] = 'Email must be valid containing an @ and .';
                        break;

                    case 'userName':
                        if (value.length < 2) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'Username must be at least 2 characters long';
                        }
                        break;

                    case 'password':
                        if (value.length < 6) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'Password must be at least 6 characters long';
                        } else if (input.confirmPassword && value !== input.confirmPassword) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'Passwords must match';
                        }
                        break;

                    case 'confirmPassword':
                        if (value.length < 6) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'Please confirm your password';
                        } else if (input.password && value !== input.password) {
                            setCustomValidityMessage = "_"
                            errState[name] = 'Passwords must match';
                        }
                        break;
                }
            }

            e.target.setCustomValidity(setCustomValidityMessage)
            return errState;
        })
    }

    const handleInvalid = (e: any) => {
        e.preventDefault();
        validateInput(e);
    }

    const handleSubmit = (e: any) => {
            e.preventDefault();
            
            const form = e.target;
            const isValid = form.checkValidity();

            if (isValid) {
                const formData = new FormData(form);
                const obfuscatedPw = obfuscate(formData.get('password') as string);
                formData.set('password', obfuscatedPw);
                formData.set('confirmPassword', obfuscatedPw);

                startTransition(async () => {
                    const res = await handleRegistering(formData);

                    if (res && !res.error) {
                        localStorage.setItem('username', res.user.userName);
                        router.push('/');
                    }
                });
            }
    };

    return (
        <form className="border-2 border-gray-400 p-10 shadow-md bg-white rounded-2xl"
            onSubmit={handleSubmit}
            onInvalid={handleInvalid}>
            <div className="float-left mb-4 mr-2">
                <label className="block">First Name:</label>
                <div className="relative">
                    <input
                        value={input.firstName}
                        name="firstName"
                        type="text"
                        placeholder="Bob"
                        onChange={onInputChange}
                        onBlur={validateInput}
                        className="peer transition-all shadow appearance-none border rounded w-full py-2 px-3
    text-gray-700 leading-tight focus:outline-none focus:shadow-outline invalid:border-pink-500 invalid:text-pink-600" />
                    {<InputErrorMessage>{error.firstName}</InputErrorMessage>}
                </div>
            </div>
            <div className="float-right mb-4">
                <label className="block">Last Name:</label>
                <div className="relative">
                    <input
                        value={input.lastName}
                        name="lastName"
                        type="text"
                        placeholder="Bobson"
                        onChange={onInputChange}
                        onBlur={validateInput}
                        className="peer transition-all shadow appearance-none border rounded w-full py-2 px-3 
    text-gray-700 leading-tight focus:outline-none focus:shadow-outline invalid:border-pink-500 invalid:text-pink-600" />
                    {<InputErrorMessage>{error.lastName}</InputErrorMessage>}
                </div>
            </div>
            <div className="mb-4">
                <label className="block">Email:</label>
                <div className="relative">
                    <input
                        value={input.email}
                        name="email"
                        type="email"
                        placeholder="bob@bob.com"
                        onChange={onInputChange}
                        onBlur={validateInput}
                        className="peer transition-all shadow appearance-none border rounded w-full py-2 px-3 
    text-gray-700 leading-tight focus:outline-none focus:shadow-outline invalid:border-pink-500 invalid:text-pink-600" />
                    {<InputErrorMessage>{error.email}</InputErrorMessage>}
                </div>
            </div>
            <div className="mb-4">
                <label className="block">Username:</label>
                <div className="relative">
                    <input
                        value={input.userName}
                        name="userName"
                        type="text"
                        placeholder="bob"
                        onChange={onInputChange}
                        onBlur={validateInput}
                        className="peer shadow appearance-none border rounded w-full py-2 px-3
    text-gray-700 leading-tight focus:outline-none focus:shadow-outline invalid:border-pink-500 invalid:text-pink-600" />
                    {<InputErrorMessage>{error.userName}</InputErrorMessage>}
                </div>
            </div>
            <div className="mb-2">
                <label className="block">Password:</label>
                <div className="relative">
                    <input
                        value={input.password}
                        name="password"
                        type="password"
                        placeholder="***********"
                        onChange={onInputChange}
                        onBlur={validateInput}
                        className="peer shadow appearance-none border rounded w-full py-2 px-3 
    text-gray-700 leading-tight focus:outline-none focus:shadow-outline invalid:border-pink-500 invalid:text-pink-600" />
                    {<InputErrorMessage>{error.password}</InputErrorMessage>}
                </div>
            </div>
            <div className="mb-10">
                <label className="block">Confirm Password:</label>
                <div className="relative">
                    <input
                        value={input.confirmPassword}
                        name="confirmPassword"
                        type="password"
                        placeholder="***********"
                        onChange={onInputChange}
                        onBlur={validateInput}
                        className="peer shadow appearance-none border rounded w-full py-2 px-3 
    text-gray-700 leading-tight focus:outline-none focus:shadow-outline invalid:border-pink-500 invalid:text-pink-600" />
                    {<InputErrorMessage>{error.confirmPassword}</InputErrorMessage>}
                </div>
            </div>
            <div className="flex justify-center">
                <button className="bg-gray-50 hover:bg-gray-100 border-gray-400 border-2 font-semibold py-3 px-4 rounded-2xl shadow-md transition
                               disabled:text-gray-300 disabled:border-gray-200 disabled:pointer-events-non"
                    type="submit" disabled={isPending}>Register</button>
            </div>
        </form>
    )
}