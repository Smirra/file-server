'use client'

import { Menu } from "@headlessui/react";
import { ChevronDownIcon } from "@heroicons/react/24/outline";

export function UserDropdown({ user }) {
    return (
        <Menu as="div" className="relative inline-block">
            <Menu.Button className="inline-flex items-center gap-x-1.5">
                {user.userName}
                <ChevronDownIcon className="h-5 w-5"/>
            </Menu.Button>
            <Menu.Items className="absolute flex flex-col mt-1.5 right-0 bg-white border-2 border-gray-400 shadow-xl rounded-xl p-2 text-left">
                <Menu.Item>
                    <a href={`/${user.userName}/profile`}>Profile</a>
                </Menu.Item>
                <Menu.Item>
                    <a href={`/${user.userName}/settings`}>Settings</a>
                </Menu.Item>
                {user.isAdmin &&
                    <Menu.Item>
                        <a href="/admin">Admin</a>
                    </Menu.Item>
                }
            </Menu.Items>
        </Menu>
    )
}