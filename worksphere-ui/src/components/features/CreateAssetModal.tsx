// src/components/features/CreateAssetModal.tsx
import { useState } from 'react'
import { useAppDispatch, useAppSelector } from '../../store/hooks'
import { createAsset, resetCreateStatus } from '../../store/slices/assetSlice'
import Modal from '../ui/Modal'

interface CreateAssetModalProps {
  isOpen: boolean
  onClose: () => void
  onCreated: () => void
}

export default function CreateAssetModal({ isOpen, onClose, onCreated }: CreateAssetModalProps) {
  const dispatch = useAppDispatch()
  const { createStatus, error } = useAppSelector((state) => state.asset)

  const [name, setName] = useState('')
  const [assetType, setAssetType] = useState('')
  const [serialNumber, setSerialNumber] = useState('')
  const [purchaseDate, setPurchaseDate] = useState('')

  const isLoading = createStatus === 'loading'

  const resetForm = () => {
    setName('')
    setAssetType('')
    setSerialNumber('')
    setPurchaseDate('')
    dispatch(resetCreateStatus())
  }

  const handleClose = () => {
    resetForm()
    onClose()
  }

  const handleSubmit = async () => {
    const result = await dispatch(
      createAsset({ name, assetType, serialNumber, purchaseDate })
    )

    if (createAsset.fulfilled.match(result)) {
      resetForm()
      onCreated()
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Yeni Demirbaş Ekle">
      <div className="mb-4">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Demirbaş Adı
        </label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
          placeholder="Dell Latitude 5420"
        />
      </div>

      <div className="mb-4">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Tür
        </label>
        <input
          type="text"
          value={assetType}
          onChange={(e) => setAssetType(e.target.value)}
          className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
          placeholder="Laptop"
        />
      </div>

      <div className="mb-4">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Seri Numarası
        </label>
        <input
          type="text"
          value={serialNumber}
          onChange={(e) => setSerialNumber(e.target.value)}
          className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
          placeholder="SN-2024-001"
        />
      </div>

      <div className="mb-5">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Satın Alma Tarihi
        </label>
        <input
          type="date"
          value={purchaseDate}
          onChange={(e) => setPurchaseDate(e.target.value)}
          className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
        />
      </div>

      {error && (
        <div className="mb-4 rounded-lg border border-red-900 bg-red-950/50 px-3 py-2 text-sm text-red-400">
          {error}
        </div>
      )}

      <div className="flex justify-end gap-3">
        <button
          onClick={handleClose}
          className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800"
        >
          Vazgeç
        </button>
        <button
          onClick={handleSubmit}
          disabled={isLoading || !name || !assetType || !serialNumber || !purchaseDate}
          className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
        >
          {isLoading ? 'Ekleniyor...' : 'Ekle'}
        </button>
      </div>
    </Modal>
  )
}
