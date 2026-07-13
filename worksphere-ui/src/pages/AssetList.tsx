// src/pages/AssetList.tsx
import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchAssets } from '../store/slices/assetSlice'
import { returnAsset, fetchActiveAssignmentByAssetId } from '../store/slices/assetAssignmentSlice'
import Table, { type Column } from '../components/ui/Table'
import AssetStatusBadge from '../components/ui/AssetStatusBadge'
import Select from '../components/ui/Select'
import AssignAssetModal from '../components/features/AssignAssetModal'
import CreateAssetModal from '../components/features/CreateAssetModal'
import type { AssetDto, AssetStatus } from '../types/asset'

const statusOptions: { value: AssetStatus; label: string }[] = [
  { value: 'Available', label: 'Müsait' },
  { value: 'Assigned', label: 'Zimmetli' },
  { value: 'UnderRepair', label: 'Tamirde' },
  { value: 'Retired', label: 'Kullanım Dışı' },
]

export default function AssetList() {
  const dispatch = useAppDispatch()
  const { items, status } = useAppSelector((state) => state.asset)

  const [searchParams, setSearchParams] = useSearchParams()
  const statusFilter = searchParams.get('status') ?? ''

  const [assigningAsset, setAssigningAsset] = useState<AssetDto | null>(null)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [returningAssetId, setReturningAssetId] = useState<string | null>(null)

  const loadAssets = () => {
    dispatch(fetchAssets(statusFilter ? { status: statusFilter as AssetStatus } : {}))
  }

  useEffect(() => {
    loadAssets()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dispatch, statusFilter])

  const handleStatusChange = (value: string) => {
    if (value) {
      setSearchParams({ status: value })
    } else {
      setSearchParams({})
    }
  }

  const handleAssigned = () => {
    setAssigningAsset(null)
    loadAssets()
  }

  const handleCreated = () => {
    setIsCreateModalOpen(false)
    loadAssets()
  }

  const handleReturn = async (asset: AssetDto) => {
    if (!window.confirm(`"${asset.name}" demirbaşını iade etmek istediğinize emin misiniz?`)) {
      return
    }

    const returnNote = window.prompt('İade notu girebilirsiniz (opsiyonel):')
    if (returnNote === null) return

    setReturningAssetId(asset.id)
    try {
      const assignment = await dispatch(fetchActiveAssignmentByAssetId(asset.id)).unwrap()
      const result = await dispatch(
        returnAsset({
          assignmentId: assignment.id,
          dto: { returnNote: returnNote.trim() || undefined },
        })
      )

      if (returnAsset.fulfilled.match(result)) {
        loadAssets()
      } else {
        alert(result.payload || 'İade işlemi başarısız oldu.')
      }
    } catch (err) {
      const errMsg = err instanceof Error ? err.message : String(err)
      alert(errMsg || 'Zimmet kaydı bulunamadı veya iade işlemi sırasında bir hata oluştu.')
    } finally {
      setReturningAssetId(null)
    }
  }

  const columns: Column<AssetDto>[] = [
    { key: 'name', header: 'Demirbaş Adı' },
    { key: 'assetType', header: 'Tür' },
    { key: 'serialNumber', header: 'Seri No' },
    {
      key: 'purchaseDate',
      header: 'Satın Alma Tarihi',
      render: (row) => new Date(row.purchaseDate).toLocaleDateString('tr-TR'),
    },
    {
      key: 'status',
      header: 'Durum',
      render: (row) => <AssetStatusBadge status={row.status} />,
    },
    {
      key: 'actions',
      header: '',
      render: (row) => (
        <div className="flex justify-end gap-2">
          {row.status === 'Available' && (
            <button
              onClick={() => setAssigningAsset(row)}
              className="rounded-lg px-3 py-1.5 text-xs font-medium text-slate-300 hover:bg-slate-800"
            >
              Zimmetle
            </button>
          )}
          {row.status === 'Assigned' && (
            <button
              onClick={() => handleReturn(row)}
              disabled={returningAssetId === row.id}
              className="rounded-lg px-3 py-1.5 text-xs font-medium text-red-400 hover:bg-red-950/30 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {returningAssetId === row.id ? 'İade ediliyor...' : 'İade Et'}
            </button>
          )}
        </div>
      ),
    },
  ]

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-white">Demirbaşlar</h1>

        <div className="flex items-center gap-3">
          <Select
            value={statusFilter}
            onChange={handleStatusChange}
            options={statusOptions}
            placeholder="Duruma göre filtrele"
          />
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200"
          >
            + Yeni Demirbaş
          </button>
        </div>
      </div>

      <div className="mt-6">
        <Table
          columns={columns}
          data={items}
          keyExtractor={(row) => row.id}
          isLoading={status === 'loading'}
          emptyMessage="Demirbaş bulunamadı."
        />
      </div>

      <AssignAssetModal
        key={assigningAsset?.id || 'none'}
        asset={assigningAsset}
        onClose={() => setAssigningAsset(null)}
        onAssigned={handleAssigned}
      />

      <CreateAssetModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onCreated={handleCreated}
      />
    </div>
  )
}