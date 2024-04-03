import { useRef } from 'react';
import { CSSTransition } from 'react-transition-group';


export default function PageAlertMessage({
    children,
    message,
    className
}: {
    message: { title?: string, body?: string };
    className?: string;
    children?: React.ReactNode;
}) {
    const nodeRef = useRef(null);

    return (
        <CSSTransition nodeRef={nodeRef} in={!!message.title && !!message.body} timeout={700} mountOnEnter classNames={{
            enterActive: 'translate-y-[0] opacity-100',
            enterDone: 'translate-y-[0] opacity-100',
        }}>
            <div ref={nodeRef} className={`${className} ease-in-out transition duration-700 translate-y-52 opacity-0 bg-red-100 border-2 border-red-400 text-center text-red-700 px-5 py-6 rounded-3xl shadow-lg`}
                role="alert" >
                <p className='pb-4'><strong>{message.title}</strong></p>
                <p>{message.body}</p>
                {children &&
                    <>
                        {children}
                    </>
                }
            </div>
        </CSSTransition>
    );
}