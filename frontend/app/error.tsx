'use client'
 
import { useEffect } from 'react'
 
export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  useEffect(() => {
    console.error(error)
  }, [error])
 
  return (
    <div className='main-window pt-20 flex flex-col items-center'>
      <h2 className='pb-4'>Something went wrong!</h2>
      <button className='w-fit bg-gray-50 hover:bg-gray-100 border-gray-400 border-2 font-semibold py-3 px-4 rounded-2xl shadow-md transition' onClick={() => reset()}>
        Try again
      </button>
    </div>
  )
}