import { useRef } from "react";
import { CSSTransition } from "react-transition-group";

export default function UploadResponse({ rsp, setRsp }) {
    const rspNodeRef = useRef(null);

    return (
    <CSSTransition nodeRef={rspNodeRef} in={rsp.show} timeout={300} unmountOnExit classNames={{
            enter: "opacity-100",
            enterDone: "opacity-100",
        }}>
            <div ref={rspNodeRef} className="transition-opacity flex ease-in-out opacity-0 duration-300 items-center w-full max-h-12 bg-green-100 border-2 shadow-md border-green-400 text-green-700 px-4 py-3 rounded-2xl">
                <strong className="grow text-center font-bold">{rsp.files?.map((x: { name: string; }) => x.name).join(', ')}</strong>
                <button onClick={() => setRsp({ ...rsp, show: false })}>
                    <svg className="w-6 h-6 text-green-500 fill-current" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                        <title>Close</title>
                        <path strokeWidth="2" strokeLinejoin="round" strokeLinecap="round" d="M14.348 5.652a.5.5 0 0 1 0 .707L9.707 10l4.641 4.641a.5.5 0 1 1-.707.707L9 10.707l-4.641 4.64a.5.5 0 0 1-.707-.707L8.293 10 3.652 5.359a.5.5 0 0 1 .707-.707L9 9.293l4.641-4.641a.5.5 0 0 1 .707 0Z" />
                    </svg>
                </button>
            </div>
        </CSSTransition>
    )
}