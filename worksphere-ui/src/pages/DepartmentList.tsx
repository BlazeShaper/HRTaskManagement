// src/pages/DepartmentList.tsx
import { useEffect, useState } from 'react'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchDepartments } from '../store/slices/departmentSlice'
import Table, { type Column } from '../components/ui/Table'
import CreateDepartmentModal from '../components/features/CreateDepartmentModal'
import type { DepartmentDto } from '../types/department'

export default function DepartmentList() {
  const dispatch = useAppDispatch()
  const { items, status } = useAppSelector((state) => state.department)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)

  const loadDepartments = () => {
    dispatch(fetchDepartments({}))
  }

  useEffect(() => {
    loadDepartments()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dispatch])

  const handleCreated = () => {
    setIsCreateModalOpen(false)
    loadDepartments()
  }

  const columns: Column<DepartmentDto>[] = [
    { key: 'name', header: 'Departman Adı' },
    { key: 'description', header: 'Açıklama' },
    {
      key: 'managerFullName',
      header: 'Yönetici',
      render: (row) => row.managerFullName ?? '—',
    },
    { key: 'employeeCount', header: 'Çalışan Sayısı' },
  ]

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-white">Departmanlar</h1>
        <button
          onClick={() => setIsCreateModalOpen(true)}
          className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200"
        >
          + Yeni Departman
        </button>
      </div>

      <div className="mt-6">
        <Table
          columns={columns}
          data={items}
          keyExtractor={(row) => row.id}
          isLoading={status === 'loading'}
          emptyMessage="Departman bulunamadı."
        />
      </div>

      <CreateDepartmentModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onCreated={handleCreated}
      />
    </div>
  )
}
