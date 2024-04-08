export default function UploadForm({ fileRefs, handleFileChange, handleSubmit }) {
    return (
        <form onSubmit={handleSubmit} className="flex max-h-72 h-2/3 gap-4 items w-full">
            <label
                className="flex flex-col w-full bg-white border-dashed border-slate-400 shadow-md
                   border-2 rounded-2xl items-center justify-center
                 hover:bg-slate-50 hover:cursor-pointer transition">
                <svg className="w-12 h-12 text-gray-400" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 20 16">
                    <path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M13 13h3a3 3 0 0 0 0-6h-.025A5.56 5.56 0 0 0 16 6.5 5.5 5.5 0 0 0 5.207 5.021C5.137 5.017 5.071 5 5 5a4 4 0 0 0 0 8h2.167M10 15V6m0 0L8 8m2-2 2 2" />
                </svg>
                <p className="text-center"><span className="font-semibold">Click here</span> or drag and drop to add a file.</p>
                <input name="file" type="file" className="hidden" onChange={handleFileChange} multiple />
            </label>
            <button disabled={fileRefs.length === 0}
                className="w-32 h-24 bg-white hover:bg-slate-50 border-slate-400 border-2 font-semibold py-2 px-4 rounded-2xl shadow-md transition
                       disabled:text-gray-300 disabled:border-slate-200 disabled:pointer-events-none">
                Upload
            </button>
        </form>
    )
}