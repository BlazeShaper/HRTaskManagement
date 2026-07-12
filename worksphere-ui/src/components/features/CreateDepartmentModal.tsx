// src/components/features/CreateDepartmentModal.tsx
import { useState } from 'react'
import { useAppDispatch, useAppSelector } from '../../store/hooks'
import { createDepartment, resetCreateStatus } from '../../store/slices/departmentSlice'
import Modal from '../ui/Modal'

interface CreateDepartmentModalProps {
  isOpen: boolean
  onClose: () => void
  onCreated: () => void
}

export default function CreateDepartmentModal({ isOpen, onClose, onCreated }: CreateDepartmentModalProps) {
  const dispatch = useAppDispatch()
  const { createStatus, error } = useAppSelector((state) => state.department)

  const [name, setName] = useState('')
  const [description, setDescription] = useState('')

  const isLoading = createStatus === 'loading'

  const resetForm = () => {
    setName('')
    setDescription('')
    dispatch(resetCreateStatus())
  }

  const handleClose = () => {
    resetForm()
    onClose()
  }

  const handleSubmit = async () => {
    const result = await dispatch(
      createDepartment({ name, description: description.trim() || undefined })
    )

    if (createDepartment.fulfilled.match(result)) {
      resetForm()
      onCreated()
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Yeni Departman Ekle">
      <div className="mb-4">
        <label className="mb-1.5 block text-sm font-medium text-slate-300">
          Departman Adı
        </label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
          placeholder="İnsan Kaynakları"
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
          disabled={isLoading || !name}
          className="rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
        >
          {isLoading ? 'Ekleniyor...' : 'Ekle'}
        </button>
      </div>
    </Modal>
  )
}
