import { useRef } from 'react';
import { CSSTransition } from 'react-transition-group';


export default function AlertMessage({
    message: { title, body, show },
    className
}: {
    message: { title?: string, body?: string, show?: boolean };
    className?: string;
}) {
    const nodeRef = useRef(null);

    return (
            <CSSTransition nodeRef={nodeRef} in={show} timeout={300} unmountOnExit classNames={{
                enter: 'ease-in-bounce scale-100',
                enterDone: 'scale-100',
                exit: 'ease-out-bounce',
            }}>
                <div ref={nodeRef} className={`${className} scale-0 transition-transform absolute w-full bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-2xl`}
                    role="alert" >
                    <strong className="font-bold">{title}</strong>
                    <p className="">{body}</p>
                </div>
            </CSSTransition>
    );
}