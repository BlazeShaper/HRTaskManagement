// src/components/ui/AssetStatusBadge.tsx
import type { AssetStatus } from '../../types/asset'

const statusConfig: Record<AssetStatus, { label: string; className: string }> = {
    Available: {
        label: 'Müsait',
        className: 'bg-emerald-950 text-emerald-400',
    },
    Assigned: {
        label: 'Zimmetli',
        className: 'bg-blue-950 text-blue-400',
    },
    UnderRepair: {
        label: 'Tamirde',
        className: 'bg-amber-950 text-amber-400',
    },
    Retired: {
        label: 'Kullanım Dışı',
        className: 'bg-slate-800 text-slate-400',
    },
}

interface AssetStatusBadgeProps {
    status: AssetStatus
}

export default function AssetStatusBadge({ status }: AssetStatusBadgeProps) {
    const config = statusConfig[status]

    return (
        <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${config.className}`}>
            {config.label}
        </span>
    )
}