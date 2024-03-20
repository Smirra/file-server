export default function InputErrorMessage({
  children,
  className = ''
}: {
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <div className={`${className} absolute w-60 opacity-0 scale-0
    peer-focus:peer-invalid:opacity-100
    peer-focus:peer-invalid:scale-100
    transition-all ease-in-bounce
    bottom-[100%] origin-bottom-left 
    bg-red-100 border border-red-400 text-red-700 px-1 py-1 rounded`} role="alert">
        <span className="block sm:inline">{children}</span>
      </div>
  )
}
