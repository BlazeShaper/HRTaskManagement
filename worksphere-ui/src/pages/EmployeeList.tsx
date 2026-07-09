// src/pages/EmployeeList.tsx
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchEmployees, deleteEmployee } from '../store/slices/employeeSlice'
import Table, { type Column } from '../components/ui/Table'
import ConfirmDialog from '../components/ui/ConfirmDialog'
import type { EmployeeDto } from '../types/employee'

export default function EmployeeList() {
    const navigate = useNavigate()
    const dispatch = useAppDispatch()
    const { items, status, deleteStatus } = useAppSelector((state) => state.employee)

    const [targetEmployee, setTargetEmployee] = useState<EmployeeDto | null>(null)

    useEffect(() => {
        dispatch(fetchEmployees({}))
    }, [dispatch])

    const handleDeleteConfirm = async () => {
        if (!targetEmployee) return

        const result = await dispatch(deleteEmployee(targetEmployee.id))

        if (deleteEmployee.fulfilled.match(result)) {
            setTargetEmployee(null)
        }
    }

    const columns: Column<EmployeeDto>[] = [
        { key: 'firstName', header: 'Ad' },
        { key: 'lastName', header: 'Soyad' },
        { key: 'email', header: 'E-posta' },
        { key: 'departmentName', header: 'Departman' },
        { key: 'positionTitle', header: 'Pozisyon' },
        {
            key: 'isActive',
            header: 'Durum',
            render: (row) => (
                <span
                    className={`rounded-full px-2 py-0.5 text-xs font-medium ${row.isActive
                            ? 'bg-emerald-950 text-emerald-400'
                            : 'bg-slate-800 text-slate-400'
                        }`}
                >
                    {row.isActive ? 'Aktif' : 'Pasif'}
                </span>
            ),
        },
        {
            key: 'actions',
            header: '',
            render: (row) => (
                <div className="flex justify-end gap-2">
                    <button
                        onClick={() => navigate(`/dashboard/employees/${row.id}/edit`)}
                        className="rounded-lg px-3 py-1.5 text-xs font-medium text-slate-300 hover:bg-slate-800"
                    >
                        Düzenle
                    </button>
                    <button
                        onClick={() => setTargetEmployee(row)}
                        className="rounded-lg px-3 py-1.5 text-xs font-medium text-red-400 hover:bg-red-950/50"
                    >
                        Sil
                    </button>
                </div>
            ),
        },
    ]

    return (
        <div>
            <h1 className="text-2xl font-bold text-white">Çalışanlar</h1>

            <div className="mt-6">
                <Table
                    columns={columns}
                    data={items}
                    keyExtractor={(row) => row.id}
                    isLoading={status === 'loading'}
                />
            </div>

            <ConfirmDialog
                isOpen={targetEmployee !== null}
                title="Çalışanı Sil"
                message={
                    targetEmployee
                        ? `${targetEmployee.firstName} ${targetEmployee.lastName} adlı çalışanı silmek istediğinize emin misiniz?`
                        : ''
                }
                onConfirm={handleDeleteConfirm}
                onCancel={() => setTargetEmployee(null)}
                isConfirming={deleteStatus === 'loading'}
            />
        </div>
    )
}