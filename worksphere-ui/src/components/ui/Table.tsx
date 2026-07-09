// src/components/ui/Table.tsx
export interface Column<T> {
    key: string
    header: string
    render?: (row: T) => React.ReactNode
}

interface TableProps<T> {
    columns: Column<T>[]
    data: T[]
    keyExtractor: (row: T) => string
    isLoading?: boolean
    emptyMessage?: string
}

export default function Table<T>({
    columns,
    data,
    keyExtractor,
    isLoading = false,
    emptyMessage = 'Kayıt bulunamadı.',
}: TableProps<T>) {
    if (isLoading) {
        return (
            <div className="rounded-xl border border-slate-800 bg-slate-900 p-8 text-center text-sm text-slate-500">
                Yükleniyor...
            </div>
        )
    }

    if (data.length === 0) {
        return (
            <div className="rounded-xl border border-slate-800 bg-slate-900 p-8 text-center text-sm text-slate-500">
                {emptyMessage}
            </div>
        )
    }

    return (
        <div className="overflow-x-auto rounded-xl border border-slate-800 bg-slate-900">
            <table className="w-full text-left text-sm">
                <thead>
                    <tr className="border-b border-slate-800">
                        {columns.map((column) => (
                            <th
                                key={column.key}
                                className="px-4 py-3 font-medium text-slate-400"
                            >
                                {column.header}
                            </th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {data.map((row) => (
                        <tr
                            key={keyExtractor(row)}
                            className="border-b border-slate-800/60 last:border-0 hover:bg-slate-800/40"
                        >
                            {columns.map((column) => (
                                <td key={column.key} className="px-4 py-3 text-slate-200">
                                    {column.render
                                        ? column.render(row)
                                        : String((row as Record<string, unknown>)[column.key] ?? '')}
                                </td>
                            ))}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}