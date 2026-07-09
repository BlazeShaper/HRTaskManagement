// src/pages/Login.tsx
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { loginUser } from '../store/slices/authSlice'

export default function Login() {
    const navigate = useNavigate()
    const dispatch = useAppDispatch()
    const { status, error } = useAppSelector((state) => state.auth)

    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')

    const isLoading = status === 'loading'

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault()

        const result = await dispatch(loginUser({ username, password }))

        if (loginUser.fulfilled.match(result)) {
            navigate('/dashboard')
        }
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4">
            <div className="w-full max-w-sm">
                <div className="mb-8 text-center">
                    <h1 className="text-2xl font-bold tracking-tight text-white">WorkSphere</h1>
                    <p className="mt-1 text-sm text-slate-500">Hesabınıza giriş yapın</p>
                </div>

                <form
                    onSubmit={handleSubmit}
                    className="rounded-xl border border-slate-800 bg-slate-900 p-6 shadow-xl"
                >
                    <div className="mb-4">
                        <label htmlFor="username" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Kullanıcı Adı
                        </label>
                        <input
                            id="username"
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            autoFocus
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-slate-500"
                            placeholder="ornek.kullanici"
                        />
                    </div>

                    <div className="mb-5">
                        <label htmlFor="password" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Şifre
                        </label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-slate-500"
                            placeholder="••••••••"
                        />
                    </div>

                    {error && (
                        <div className="mb-4 rounded-lg border border-red-900 bg-red-950/50 px-3 py-2 text-sm text-red-400">
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading}
                        className="w-full rounded-lg bg-white py-2 text-sm font-medium text-slate-900 transition-colors hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                        {isLoading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
                    </button>
                </form>
            </div>
        </div>
    )
}