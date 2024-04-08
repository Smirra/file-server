'use client'

import { createRef, useRef, useState } from "react";
import UploadResponse from "./upload-response";
import FileList from "./file-list";
import UploadForm from "./upload-form";

let fileId = 0;

export default function Page() {
    const [fileRefs, setFileRefs] = useState([])
    const [rsp, setRsp] = useState({files: null, show: false});

    const handleFileChange = ({ currentTarget: { files } }: React.FormEvent<HTMLInputElement>) => {
        setRsp({...rsp, show: false});
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
            setRsp({files: rspObj, show: true})
        }
    }

    return (
        <div className="w-5/6 max-w-[60em] h-full flex flex-col gap-4">
            <UploadForm fileRefs={fileRefs} handleFileChange={handleFileChange} handleSubmit={handleSubmit} />
            <FileList fileRefs={fileRefs} handleNameChange={handleNameChange} handleDelete={handleDelete} />
            <UploadResponse rsp={rsp} setRsp={setRsp} />
        </div >
    )
}