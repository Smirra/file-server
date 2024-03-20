import { createContext, useEffect, useReducer } from "react";

export const FilesContext = createContext(null);

export function FilesProvider({ children }) {
    const [state, dispatch] = useReducer(filesReducer,
        {
            fileStates: [],
            showDeleteCheckboxes: false,
            hasNoFiles: false,
        });

    useEffect(() => {
        const fetchFiles = async () => {
            const res = await fetch(`${process.env.NEXT_PUBLIC_BASEPATH_FILES}/info`,
                {
                    method: 'GET',
                    credentials: 'include',
                    headers: {
                        Accept: 'application/json',
                    },
                })

            const initFiles = await res.json()

            if (initFiles.length > 0) {
                dispatch({
                    initFiles,
                    type: 'INITIALIZE_FILES',
                })
            } else {
                dispatch({
                    type: 'HAS_NO_FILES',
                })
            }
        }

        fetchFiles()
            .catch(error => console.error(error))

    }, []);

    return (
        <FilesContext.Provider value={{ state, dispatch }}>
            {children}
        </FilesContext.Provider>
    )
}

function filesReducer(state, action) {
    switch (action.type) {
        case 'INITIALIZE_FILES':
            return {
                fileStates: action.initFiles.map((file: any) => ({ file, deleteChecked: false })),
                showDeleteCheckboxes: false,
            }
        case 'TOGGLE_DELETE_CHECKBOX':
            return {
                ...state,
                fileStates: state.fileStates.map((fileState: { file: { id: any } }) => {
                    if (fileState.file.id === action.id) {
                        return {
                            ...fileState,
                            deleteChecked: action.checked
                        };
                    } else {
                        return fileState;
                    }
                }),
            }
        case 'DELETE_FILES':
            return {
                ...state,
                fileStates: state.fileStates.filter((fileState: { file: { id: any } }) =>
                    !action.deletedFileIds.includes(fileState.file.id))
            }
        case 'SHOW_DELETE_CHECKBOXES':
            return {
                ...state,
                showDeleteCheckboxes: !state.showDeleteCheckboxes
            }
        case 'HAS_NO_FILES':
            return {
                ...state,
                hasNoFiles: true,
            }
        default:
            throw new Error('Unknown action type: ' + action.type);
    }
}