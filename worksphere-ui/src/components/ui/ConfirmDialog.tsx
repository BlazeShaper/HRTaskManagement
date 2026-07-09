// src/components/ui/ConfirmDialog.tsx
import Modal from './Modal'

interface ConfirmDialogProps {
    isOpen: boolean
    title: string
    message: string
    onConfirm: () => void
    onCancel: () => void
    isConfirming?: boolean
}

export default function ConfirmDialog({
    isOpen,
    title,
    message,
    onConfirm,
    onCancel,
    isConfirming = false,
}: ConfirmDialogProps) {
    return (
        <Modal isOpen={isOpen} onClose={onCancel} title={title}>
            <p className="text-sm text-slate-400">{message}</p>

            <div className="mt-6 flex justify-end gap-3">
                <button
                    onClick={onCancel}
                    className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-300 hover:bg-slate-800"
                >
                    Vazgeç
                </button>
                <button
                    onClick={onConfirm}
                    disabled={isConfirming}
                    className="rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    {isConfirming ? 'Siliniyor...' : 'Sil'}
                </button>
            </div>
        </Modal>
    )
}