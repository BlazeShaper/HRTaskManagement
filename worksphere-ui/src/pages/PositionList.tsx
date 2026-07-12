// src/pages/PositionList.tsx
import { useEffect, useState } from 'react'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchPositions } from '../store/slices/positionSlice'
import Table, { type Column } from '../components/ui/Table'
import CreatePositionModal from '../components/features/CreatePositionModal'
import type { PositionDto } from '../types/position'

export default function PositionList() {
  const dispatch = useAppDispatch()
  const { items, status } = useAppSelector((state) => state.position)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)

  const loadPositions = () => {
    dispatch(fetchPositions({}))
  }

  useEffect(() => {
    loadPositions()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dispatch])

  const handleCreated = () => {
    setIsCreateModalOpen(false)
    loadPositions()
  }

  const columns: Column<PositionDto>[] = [
    { key: 'title', header: 'Pozisyon Adı' },
    { key: 'description', header: 'Açıklama' },
    { key: 'employeeCount', header: 'Çalışan Sayısı' },
  ]

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-white">Pozisyonlar</h1>
        <button
          onClick={() => setIsCreateModalOpen(true)}
          className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200"
        >
          + Yeni Pozisyon
        </button>
      </div>

      <div className="mt-6">
        <Table
          columns={columns}
          data={items}
          keyExtractor={(row) => row.id}
          isLoading={status === 'loading'}
          emptyMessage="Pozisyon bulunamadı."
        />
      </div>

      <CreatePositionModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onCreated={handleCreated}
      />
    </div>
  )
}
