import { useContext } from "react";
import { FilesContext } from "./files-context";

export default function NoFiles() {
    const { state } = useContext(FilesContext);
    return (
        <>
            {state.hasNoFiles &&
                <div className="flex flex-col justify-center items-center h-full">
                    <p className="text-2xl font-medium mb-4">No files found</p>
                    <p className="text-xl">Upload some files to get started</p>
                </div>
            }
        </>
    )
}