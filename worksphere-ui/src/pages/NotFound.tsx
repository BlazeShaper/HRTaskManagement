import { Link } from 'react-router-dom'
import { FileQuestion, Home } from 'lucide-react'

export default function NotFound() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-slate-950 text-white px-4">
      <div className="max-w-md text-center space-y-6">
        <div className="flex justify-center">
          <div className="relative">
            <div className="absolute inset-0 bg-indigo-500 rounded-full blur-xl opacity-20 animate-pulse"></div>
            <div className="relative bg-slate-900 border border-slate-800 p-6 rounded-2xl shadow-2xl">
              <FileQuestion className="w-16 h-16 text-indigo-400" />
            </div>
          </div>
        </div>
        
        <div className="space-y-2">
          <h1 className="text-6xl font-extrabold tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-indigo-400 via-purple-400 to-pink-400">
            404
          </h1>
          <h2 className="text-2xl font-bold tracking-tight">Sayfa Bulunamadı</h2>
          <p className="text-slate-400 text-sm max-w-xs mx-auto">
            Aradığınız sayfa kaldırılmış, adı değiştirilmiş veya geçici olarak kullanım dışı kalmış olabilir.
          </p>
        </div>

        <div>
          <Link
            to="/dashboard"
            className="inline-flex items-center justify-center gap-2 bg-gradient-to-r from-indigo-500 to-purple-600 hover:from-indigo-600 hover:to-purple-700 text-white font-medium px-6 py-3 rounded-xl shadow-lg shadow-indigo-500/20 transition-all duration-200 transform hover:-translate-y-0.5"
          >
            <Home className="w-4 h-4" />
            Panele Dön
          </Link>
        </div>
      </div>
    </div>
  )
}
