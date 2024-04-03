'use client'

import { FilesProvider } from "./files-context";
import MultiselectMenu from "./multiselect-menu";
import FileList from "./file-list";
import NoFiles from "./no-files";
import BadRsp from "./bad-rsp";

export default function Page() {

    return (
        <FilesProvider>
            <div className="h-fit grid w-2/3 row-auto gap-5">
                <MultiselectMenu />
                <FileList />
                <NoFiles />
                <BadRsp />
            </div>
        </FilesProvider>
    )
}