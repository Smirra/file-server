import { useContext } from "react";
import { FilesContext } from "./files-context";

export default function MultiselectMenu() {
    const { state, dispatch } = useContext(FilesContext);

    const onCheckedDeleteCheckbox = state.fileStates.some((item: { deleteChecked: boolean; }) => item.deleteChecked);

    const handleDeleteFiles = async () => {
        const ids = state.fileStates.reduce((acc: string[], fileState: { file: { id: string; }; deleteChecked: boolean; }) =>
            fileState.deleteChecked ? (acc.push(fileState.file.id), acc) : acc, []);
        const res = await fetch(`${process.env.NEXT_PUBLIC_BASEPATH_FILES}/?ids=${ids}`,
            {
                method: 'DELETE',
                credentials: 'include',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                }
            })

        const rspObj = await res.json();
        const deletedFileIds = rspObj.deleted;

        dispatch({
            deletedFileIds,
            type: 'DELETE_FILES',
        })
    }

    return (
        <>
            {state.fileStates.length > 0 &&
                <div className="flex flex-row w-full justify-end">
                    {!onCheckedDeleteCheckbox &&
                        <button
                            className="bg-white p-2 border-gray-400 rounded-2xl border-2 shadow-md hover:bg-gray-100 hover:scale-105 transition ease-in-out"
                            onClick={() => dispatch({
                                type: 'SHOW_DELETE_CHECKBOXES'
                            })}>
                            <svg className="w-5 h-5 text-gray-800 dark:text-white" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 18 20">
                                <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 5h16M7 8v8m4-8v8M7 1h4a1 1 0 0 1 1 1v3H6V2a1 1 0 0 1 1-1ZM3 5h12v13a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V5Z" />
                            </svg>
                        </button>}
                    {onCheckedDeleteCheckbox &&
                        <button
                            className="bg-red-50 p-2 border-red-400 rounded-2xl border-2 shadow-md hover:bg-red-100 scale-105 hover:scale-110 transition ease-in-out"
                            onClick={handleDeleteFiles}>
                            <svg className="w-5 h-5 text-red-800 dark:text-white" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 18 20">
                                <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 5h16M7 8v8m4-8v8M7 1h4a1 1 0 0 1 1 1v3H6V2a1 1 0 0 1 1-1ZM3 5h12v13a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V5Z" />
                            </svg>
                        </button>}
                </div>
            }
        </>
    )
}