// src/components/ui/Modal.tsx
import type { ReactNode } from 'react'

interface ModalProps {
    isOpen: boolean
    onClose: () => void
    title: string
    children: ReactNode
}

export default function Modal({ isOpen, onClose, title, children }: ModalProps) {
    if (!isOpen) return null

    return (
        <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 px-4"
            onClick={onClose}
        >
            <div
                className="w-full max-w-md rounded-xl border border-slate-800 bg-slate-900 p-6 shadow-xl"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between">
                    <h2 className="text-lg font-semibold text-white">{title}</h2>
                    <button
                        onClick={onClose}
                        className="rounded-lg p-1 text-slate-400 hover:bg-slate-800 hover:text-white"
                    >
                        ✕
                    </button>
                </div>
                <div className="mt-3">{children}</div>
            </div>
        </div>
    )
}