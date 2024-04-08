import { TransitionGroup, CSSTransition } from "react-transition-group";

export default function FileList({ fileRefs, handleNameChange, handleDelete }) {
    return (
        <TransitionGroup>
            {fileRefs.map(({ file, nodeRef, id }) => (
                <CSSTransition nodeRef={nodeRef} key={id} timeout={300} classNames={{
                    enter: "max-h-[0] h-[0] border-y-0 my-[0]",
                    exit: "max-h-[0] h-[0] border-y-0 my-[0]",
                }}>
                    <FileItem file={file} nodeRef={nodeRef} handleNameChange={handleNameChange} handleDelete={handleDelete} />
                </CSSTransition>
            ))}
        </TransitionGroup>
    )
}

function FileItem({ file, nodeRef, handleNameChange, handleDelete }) {
    return (
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
    )
}