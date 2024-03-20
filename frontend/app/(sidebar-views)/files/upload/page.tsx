'use client'

import { createRef, useRef, useState } from "react";
import { TransitionGroup, CSSTransition } from "react-transition-group";

let fileId = 0;

export default function Page() {
    const [fileRefs, setFileRefs] = useState([])
    const [rsp, setRsp] = useState(null);
    const rspNodeRef = useRef(null);

    const handleFileChange = ({ currentTarget: { files } }: React.FormEvent<HTMLInputElement>) => {
        setRsp(null);
        const newFileRefs = [...files].map(file => ({ file, id: fileId++, name: file.name, nodeRef: createRef() }));

        setFileRefs(prev => [...prev, ...newFileRefs]);
    }

    const handleNameChange = (currentFile: File, newName: string) => {
        setFileRefs(prev =>
            prev.map(fileRef => ({
                ...fileRef,
                name: fileRef.file === currentFile ? newName : fileRef.name
            }))
        );
    };

    const handleDelete = (currentFile: File) => {
        setFileRefs(prev => prev.filter(({ file }) => file !== currentFile))
    }

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault()

        const formData = new FormData()
        fileRefs.forEach(({ file, name }) => formData.append('file', file, name))

        const res = await fetch(`${process.env.NEXT_PUBLIC_BASEPATH_FILES}`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                Accept: 'application/json',
            },
            body: formData
        })

        const rspObj = await res.json()

        if (res.ok) {
            setFileRefs([])
            setRsp(rspObj)
        }
    }

    return (
        <div className="h-fit w-[calc(100vh*2/3)] flex flex-col gap-4">
            <form onSubmit={handleSubmit} className="flex gap-4 items-end w-full">
                <label
                    className="flex flex-col w-full bg-white h-64 border-dashed border-slate-400 shadow-md
                           border-2 rounded-2xl items-center justify-center
                         hover:bg-slate-50 hover:cursor-pointer transition">
                    <svg className="w-12 h-12 text-gray-400" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 16">
                        <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M13 13h3a3 3 0 0 0 0-6h-.025A5.56 5.56 0 0 0 16 6.5 5.5 5.5 0 0 0 5.207 5.021C5.137 5.017 5.071 5 5 5a4 4 0 0 0 0 8h2.167M10 15V6m0 0L8 8m2-2 2 2" />
                    </svg>
                    <p><span className="font-semibold">Click to add</span> or drag and drop a file.</p>
                    <input name="file" type="file" className="hidden" onChange={handleFileChange} multiple />
                </label>
                <button disabled={fileRefs.length === 0}
                    className="w-32 h-24 bg-white hover:bg-slate-50 border-slate-400 border-2 font-semibold py-2 px-4 rounded-2xl shadow-md transition
                               disabled:text-gray-300 disabled:border-slate-200 disabled:pointer-events-none">
                    Upload
                </button>
            </form>
            <TransitionGroup>
                {fileRefs.map(({ file, nodeRef, id }) => (
                    <CSSTransition nodeRef={nodeRef} key={id} timeout={300} classNames={{
                        enter: "max-h-[0] h-[0] border-t-[0] border-b-[0] my-[0px]",
                        exit: "max-h-[0] h-[0] border-t-[0] border-b-[0] my-[0px]",
                    }}>
                        <div className="pb-[1px]"> {/* hack to prevent margin-collapsing */}
                            <div ref={nodeRef} className="my-2 flex transition-all ease-in-out duration-300 items-center justify-between px-4 h-14 max-h-14 overflow-hidden bg-white border-gray-400 rounded-2xl border-2 shadow-md">
                                <input
                                    type="text"
                                    onChange={(e) => handleNameChange(file, e.target.value)}
                                    className="w-[32em] max-h-6 underline decoration-dotted underline-offset-3 hover:text-sky-700 decoration-gray-400 rounded-md focus:outline-dashed focus:no-underline focus:text-black focus:hover:outline-gray-300 focus:outline-gray-200 focus:outline-2 focus:outline-offset-4"
                                    defaultValue={file.name}
                                />
                                <button
                                    onClick={() => handleDelete(file)}>
                                    <svg className="w-7 h-5 text-gray-800 hover:text-gray-700 hover:scale-110 duration-100 transition" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 18 20">
                                        <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 5h16M7 8v8m4-8v8M7 1h4a1 1 0 0 1 1 1v3H6V2a1 1 0 0 1 1-1ZM3 5h12v13a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V5Z" />
                                    </svg>
                                </button>
                            </div>
                        </div>
                    </CSSTransition>
                ))}
            </TransitionGroup>
            <CSSTransition nodeRef={rspNodeRef} in={rsp !== null} timeout={300} unmountOnExit classNames={{
                enterActive: "opacity-100",
                enterDone: "opacity-100",
                exit: "opacity-0",
            }}>
                <div ref={rspNodeRef} className="transition-opacity flex ease-in-out opacity-0 duration-300 items-center w-full max-h-12 bg-green-100 border-2 shadow-md border-green-400 text-green-700 px-4 py-3 rounded-2xl">
                    <strong className="grow text-center font-bold">{rsp?.map(x => x.name).join(', ')}</strong>
                    <button onClick={() => setRsp(null)}>
                        <svg className="w-6 h-6 text-green-500 fill-current" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                            <title>Close</title>
                            <path strokeWidth="2" strokeLinejoin="round" strokeLinecap="round" d="M14.348 5.652a.5.5 0 0 1 0 .707L9.707 10l4.641 4.641a.5.5 0 1 1-.707.707L9 10.707l-4.641 4.64a.5.5 0 0 1-.707-.707L8.293 10 3.652 5.359a.5.5 0 0 1 .707-.707L9 9.293l4.641-4.641a.5.5 0 0 1 .707 0Z" />
                        </svg>
                    </button>
                </div>
            </CSSTransition>
        </div >
    )
}