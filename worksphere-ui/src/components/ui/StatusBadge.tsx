// src/components/ui/StatusBadge.tsx
import type { TaskStatus } from '../../types/task'

const statusConfig: Record<TaskStatus, { label: string; className: string }> = {
    Pending: {
        label: 'Beklemede',
        className: 'bg-slate-800 text-slate-300',
    },
    InProgress: {
        label: 'Devam Ediyor',
        className: 'bg-blue-950 text-blue-400',
    },
    Completed: {
        label: 'Tamamlandı',
        className: 'bg-emerald-950 text-emerald-400',
    },
    Cancelled: {
        label: 'İptal Edildi',
        className: 'bg-red-950 text-red-400',
    },
}

interface StatusBadgeProps {
    status: TaskStatus
}

export default function StatusBadge({ status }: StatusBadgeProps) {
    const config = statusConfig[status]

    return (
        <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${config.className}`}>
            {config.label}
        </span>
    )
}