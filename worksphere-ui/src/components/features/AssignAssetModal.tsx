// src/components/features/AssignAssetModal.tsx
import { useState, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../../store/hooks'
import { fetchEmployees } from '../../store/slices/employeeSlice'
import { assignAsset, resetAssignStatus } from '../../store/slices/assetAssignmentSlice'
import Modal from '../ui/Modal'
import Select from '../ui/Select'
import type { AssetDto } from '../../types/asset'

interface AssignAssetModalProps {
    asset: AssetDto | null
    onClose: () => void
    onAssigned: () => void
}

export default function AssignAssetModal({ asset, onClose, onAssigned }: AssignAssetModalProps) {
    const dispatch = useAppDispatch()
    const { items: employees } = useAppSelector((state) => state.employee)
    const { assignStatus, error } = useAppSelector((state) => state.assetAssignment)

    const [employeeId, setEmployeeId] = useState('')
    const [note, setNote] = useState('')

    useEffect(() => {
        if (asset) {
            dispatch(fetchEmployees({ isActive: true }))
            dispatch(resetAssignStatus())
        }
    }, [asset, dispatch])

    const employeeOptions = employees.map((e) => ({
        value: e.id,
        label: `${e.firstName} ${e.lastName}`,
    }))

    const handleSubmit = async () => {
        if (!asset || !employeeId) return

        const result = await dispatch(
            assignAsset({ assetId: asset.id, employeeId, note: note.trim() || undefined })
        )

        if (assignAsset.fulfilled.match(result)) {
            onAssigned()
        }
    }

    if (!asset) return null

    return (
        <Modal isOpen={!!asset} onClose={onClose} title={`"${asset.name}" Demirbaşını Zimmetle`}>
            <div className="mb-4">
                <label className="mb-1.5 block text-sm font-medium text-slate-300">
                    Çalışan
                </label>
                <Select
                    value={employeeId}
                    onChange={setEmployeeId}
                    options={employeeOptions}
                    placeholder="Bir çalışan seçin"
                />
            </div>

            <div className="mb-4">
                <label className="mb-1.5 block text-sm font-medium text-slate-300">
                    Not <span className="text-slate-500">(opsiyonel)</span>
                </label>
                <textarea
                    value={note}
                    onChange={(e) => setNote(e.target.value)}
                    rows={2}
                    className="w-full resize-none rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
                />
            </div>

            {error && (
                <div className="mb-4 rounded-lg border border-red-900 bg-red-950/50 px-3 py-2 text-sm text-red-400">
                    {error}
                </div>
            )}

            <div className="flex justify-end gap-3">
                <button
                    onClick={onClose}
                    className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800"
                >
                    Vazgeç
                </button>
                <button
                    onClick={handleSubmit}
                    disabled={!employeeId || assignStatus === 'loading'}
                    className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    {assignStatus === 'loading' ? 'Zimmetleniyor...' : 'Zimmetle'}
                </button>
            </div>
        </Modal>
    )
}