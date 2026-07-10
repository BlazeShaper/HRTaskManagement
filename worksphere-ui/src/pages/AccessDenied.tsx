// src/pages/AccessDenied.tsx
import { Link } from 'react-router-dom'
import { ShieldOff } from 'lucide-react'

export default function AccessDenied() {
    return (
        <div className="flex h-screen flex-col items-center justify-center bg-slate-950 text-center">
            <ShieldOff size={48} className="text-red-500" />
            <h1 className="mt-4 text-2xl font-bold text-white">Erişim Engellendi</h1>
            <p className="mt-2 text-sm text-slate-400">
                Bu sayfayı görüntülemek için gerekli yetkiye sahip değilsiniz.
            </p>
            <Link
                to="/dashboard"
                className="mt-6 rounded-lg bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-200"
            >
                Panele Dön
            </Link>
        </div>
    )
}