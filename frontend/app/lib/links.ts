export type Links = {
    admin: { path: string; label: string; }[];
    files: { path: string; label: string; }[];
};

export const sidebarLinks: Links = {
    admin: [
        {
            path: "/admin/users",
            label: "Users",
        },
    ],
    files: [
        {
            path: "/files",
            label: "Files",
        },
        {
            path: "/files/upload",
            label: "Upload",
        },
    ],
};
