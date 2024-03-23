export default function AlertMessage({ message: { title, rspMsg } }) {
    if (rspMsg) {
        return (
            <div className="absolute w-full -top-20 shift bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded" role="alert">
                {title && <strong className="font-bold">{title}</strong>}
                <p className="">{rspMsg}</p>
            </div>
        );
    }

    return null;
}