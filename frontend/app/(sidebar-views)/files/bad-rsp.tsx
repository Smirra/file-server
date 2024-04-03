import { useContext } from "react";
import { FilesContext } from "./files-context";
import PageAlertMessage from "@/app/ui/page-alert-message";

export default function BadRsp() {
    const { state: { badRsp } } = useContext(FilesContext);

    return (
        <>
            {badRsp &&
                <PageAlertMessage message={{ title: badRsp.status, body: badRsp.statusText }} className="place-self-center max-w-[24em] w-1/2 text-2xl">
                    <button onClick={() => location.reload()} className="mt-8 bg-red-200 hover:bg-red-300 hover:scale-105 hover:-translate-y-0.5 hover:shadow-md active:scale-100 active:translate-y-0 active:shadow text-red-700 font-bold py-2 px-4 border-2 border-red-400 shadow rounded-lg">Retry</button>
                </PageAlertMessage>
            }
        </>
    )
}