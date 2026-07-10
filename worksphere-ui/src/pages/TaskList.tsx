// src/pages/TaskList.tsx
import { useEffect } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchTasks } from '../store/slices/taskSlice'
import Table, { type Column } from '../components/ui/Table'
import StatusBadge from '../components/ui/StatusBadge'
import Select from '../components/ui/Select'
import type { TaskDto, TaskStatus } from '../types/task'

const statusOptions: { value: TaskStatus; label: string }[] = [
    { value: 'Pending', label: 'Beklemede' },
    { value: 'InProgress', label: 'Devam Ediyor' },
    { value: 'Completed', label: 'Tamamlandı' },
    { value: 'Cancelled', label: 'İptal Edildi' },
]

export default function TaskList() {
    const dispatch = useAppDispatch()
    const { items, status } = useAppSelector((state) => state.task)

    const [searchParams, setSearchParams] = useSearchParams()
    const statusFilter = searchParams.get('status') ?? ''

    useEffect(() => {
        dispatch(fetchTasks(statusFilter ? { status: statusFilter as TaskStatus } : {}))
    }, [dispatch, statusFilter])

    const handleStatusChange = (value: string) => {
        if (value) {
            setSearchParams({ status: value })
        } else {
            setSearchParams({})
        }
    }

    const columns: Column<TaskDto>[] = [
        { key: 'title', header: 'Başlık' },
        { key: 'employeeFullName', header: 'Atanan Kişi' },
        {
            key: 'dueDate',
            header: 'Son Tarih',
            render: (row) => new Date(row.dueDate).toLocaleDateString('tr-TR'),
        },
        {
            key: 'status',
            header: 'Durum',
            render: (row) => <StatusBadge status={row.status} />,
        },
    ]

    return (
        <div>
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-white">Görevler</h1>

                <Select
                    value={statusFilter}
                    onChange={handleStatusChange}
                    options={statusOptions}
                    placeholder="Duruma göre filtrele"
                />
            </div>

            <div className="mt-6">
                <Table
                    columns={columns}
                    data={items}
                    keyExtractor={(row) => row.id}
                    isLoading={status === 'loading'}
                    emptyMessage="Görev bulunamadı."
                />
            </div>
        </div>
    )
}