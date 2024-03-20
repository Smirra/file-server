import { useContext, useState } from "react";
import { FilesContext } from "./files-context";
import clsx from "clsx";
import prettyBytes from "pretty-bytes";

export default function FileList() {
    const { state } = useContext(FilesContext);
    return (
        <>
            {state.fileStates.map((fileState: any) => (
                <FileItem
                    key={fileState.file.id}
                    fileState={fileState}
                />
            ))}
        </>
    )
}

function FileItem({ fileState }) {
    const { state, dispatch } = useContext(FilesContext);
    const [open, setOpen] = useState(false);

    const file = fileState.file;
    const showDeleteCheckboxes = state.showDeleteCheckboxes;

    return (
        <div className={clsx(
            "ease-in-out duration-500 overflow-hidden bg-white border-gray-400 rounded-2xl border-2 shadow-md",
            {
                "max-h-12": !open,
                "max-h-48": open,
            }
        )}>
            <div className="relative flex flex-row justify-end h-12 items-center">
                <button
                    onClick={() => setOpen(!open)}
                    className={clsx(
                        `absolute rounded-b-2xl transition justify-between flex-row duration-300 flex h-full w-full px-3
                         items-center hover:bg-gray-100 hover:bg-opacity-80`,
                        {
                            "pr-12": showDeleteCheckboxes,
                        }
                    )}>
                    <p className="text-xl">{file.name}</p>
                    <p className="text-sm z-40">{new Date(file.lastModifiedAt).toLocaleString('en-GB')}</p>
                </button>
                {showDeleteCheckboxes && <input type="checkbox"
                    checked={fileState.deleteChecked}
                    onChange={e => dispatch({
                        checked: e.target.checked,
                        id: file.id,
                        type: 'TOGGLE_DELETE_CHECKBOX',
                    })}
                    className="z-40 ml-6 h-5 w-5 mr-3" />}
            </div>
            <div className="flex w-full pt-1 pb-2 px-3 justify-between">
                <div className="grid grid-cols-2 gap-1 w-3/4">
                    <p className="text-sm">ID: {file.id}</p>
                    <p className="text-sm">Size: {prettyBytes(file.size)}</p>
                    <p className="text-sm">Type: {file.contentType}</p>
                    <p className="text-sm">Modified: {new Date(file.uploadedAt).toLocaleString('en-GB')}</p>
                    <p className="text-sm">Checksum: {file.checkSum}</p>
                </div>
                <div className="flex items-center justify-center px-2">
                    <a className="bg-gray-50 hover:bg-gray-100 border-gray-400 border-2 font-semibold py-3 px-4 rounded-2xl shadow-md transition"
                        href={`${process.env.NEXT_PUBLIC_BASEPATH_FILES}/download/${file.id}`}>Download</a>
                </div>
            </div>
        </div>
    )
}