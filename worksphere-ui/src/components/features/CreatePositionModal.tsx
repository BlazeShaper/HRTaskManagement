// src/components/features/CreatePositionModal.tsx
import { useState } from 'react'
import { useAppDispatch, useAppSelector } from '../../store/hooks'
import { createPosition, resetCreateStatus } from '../../store/slices/positionSlice'
import Modal from '../ui/Modal'

interface CreatePositionModalProps {
  isOpen: boolean
  onClose: () => void
  onCreated: () => void
}

export default function CreatePositionModal({ isOpen, onClose, onCreated }: CreatePositionModalProps) {
  const dispatch = useAppDispatch()
  const { createStatus, error } = useAppSelector((state) => state.position)

  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')

  const isLoading = createStatus === 'loading'

  const resetForm = () => {
    setTitle('')
    setDescription('')
    dispatch(resetCreateStatus())
  }

  const handleClose = () => {
    resetForm()
    onClose()
  }

  const handleSubmit = async () => {
    const result = await dispatch(
      createPosition({ title, description: description.trim() || undefined })
    )

    if (createPosition.fulfilled.match(result)) {
      resetForm()
      onCreated()
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Yeni Pozisyon Ekle">
      <div className="mb-4">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Pozisyon Adı
        </label>
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
          placeholder="Yazılım Geliştirici"
        />
      </div>

      <div className="mb-5">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Açıklama <span className="text-slate-500">(opsiyonel)</span>
        </label>
        <textarea
          value={description}
          onChange={(e) => setDescription(e.target.value)}
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
          onClick={handleClose}
          className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800"
        >
          Vazgeç
        </button>
        <button
          onClick={handleSubmit}
          disabled={isLoading || !title}
          className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
        >
          {isLoading ? 'Ekleniyor...' : 'Ekle'}
        </button>
      </div>
    </Modal>
  )
}
