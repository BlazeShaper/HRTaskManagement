// src/pages/AssetList.tsx
import { useEffect } from 'react'
import { useSearchParams } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchAssets } from '../store/slices/assetSlice'
import Table, { type Column } from '../components/ui/Table'
import AssetStatusBadge from '../components/ui/AssetStatusBadge'
import Select from '../components/ui/Select'
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

    useEffect(() => {
        dispatch(fetchAssets(statusFilter ? { status: statusFilter as AssetStatus } : {}))
    }, [dispatch, statusFilter])

    const handleStatusChange = (value: string) => {
        if (value) {
            setSearchParams({ status: value })
        } else {
            setSearchParams({})
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
    ]

    return (
        <div>
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-white">Demirbaşlar</h1>

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
                    emptyMessage="Demirbaş bulunamadı."
                />
            </div>
        </div>
    )
}