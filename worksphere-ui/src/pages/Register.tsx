// src/pages/Register.tsx
import { useState, type FormEvent } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { registerRequest } from '../api/authApi'
import type { RegisterRequest } from '../types/auth'

const ROLES = [
    { value: 'Admin', label: 'Admin' },
    { value: 'Manager', label: 'Yönetici' },
    { value: 'HR', label: 'İnsan Kaynakları' },
    { value: 'Employee', label: 'Çalışan' },
]

export default function Register() {
    const navigate = useNavigate()

    const [form, setForm] = useState<RegisterRequest>({
        username: '',
        email: '',
        password: '',
        role: 'Employee',
    })
    const [confirmPassword, setConfirmPassword] = useState('')
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }))
    }

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault()
        setError(null)

        if (form.password !== confirmPassword) {
            setError('Şifreler eşleşmiyor.')
            return
        }

        if (form.password.length < 6) {
            setError('Şifre en az 6 karakter olmalıdır.')
            return
        }

        setIsLoading(true)

        try {
            await registerRequest(form)
            setSuccess(true)
            setTimeout(() => navigate('/login'), 2000)
        } catch (err: unknown) {
            if (err && typeof err === 'object' && 'response' in err) {
                const axiosErr = err as { response?: { data?: { message?: string } } }
                setError(axiosErr.response?.data?.message || 'Kayıt sırasında bir hata oluştu.')
            } else {
                setError('Kayıt sırasında bir hata oluştu.')
            }
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4">
            <div className="w-full max-w-sm">
                <div className="mb-8 text-center">
                    <h1 className="text-2xl font-bold tracking-tight text-white">WorkSphere</h1>
                    <p className="mt-1 text-sm text-slate-500">Yeni hesap oluşturun</p>
                </div>

                <form
                    onSubmit={handleSubmit}
                    className="rounded-xl border border-slate-800 bg-slate-900 p-6 shadow-xl"
                >
                    <div className="mb-4">
                        <label htmlFor="reg-username" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Kullanıcı Adı
                        </label>
                        <input
                            id="reg-username"
                            name="username"
                            type="text"
                            value={form.username}
                            onChange={handleChange}
                            required
                            autoFocus
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-slate-500"
                            placeholder="ornek.kullanici"
                        />
                    </div>

                    <div className="mb-4">
                        <label htmlFor="reg-email" className="mb-1.5 block text-sm font-medium text-slate-300">
                            E-posta
                        </label>
                        <input
                            id="reg-email"
                            name="email"
                            type="email"
                            value={form.email}
                            onChange={handleChange}
                            required
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-slate-500"
                            placeholder="ornek@sirket.com"
                        />
                    </div>

                    <div className="mb-4">
                        <label htmlFor="reg-role" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Rol
                        </label>
                        <select
                            id="reg-role"
                            name="role"
                            value={form.role}
                            onChange={handleChange}
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
                        >
                            {ROLES.map((r) => (
                                <option key={r.value} value={r.value}>
                                    {r.label}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="mb-4">
                        <label htmlFor="reg-password" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Şifre
                        </label>
                        <input
                            id="reg-password"
                            name="password"
                            type="password"
                            value={form.password}
                            onChange={handleChange}
                            required
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white placeholder-slate-500 outline-none focus:border-slate-500"
                            placeholder="••••••••"
                        />
                    </div>

                    <div className="mb-5">
                        <label htmlFor="reg-confirm-password" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Şifre Tekrar
                        </label>
                        <input
                            id="reg-confirm-password"
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
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

                    {success && (
                        <div className="mb-4 rounded-lg border border-emerald-900 bg-emerald-950/50 px-3 py-2 text-sm text-emerald-400">
                            Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading || success}
                        className="w-full rounded-lg bg-white py-2 text-sm font-medium text-slate-900 transition-colors hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                        {isLoading ? 'Kayıt yapılıyor...' : 'Kayıt Ol'}
                    </button>

                    <p className="mt-4 text-center text-sm text-slate-500">
                        Zaten hesabınız var mı?{' '}
                        <Link to="/login" className="text-white underline underline-offset-2 hover:text-slate-300">
                            Giriş Yap
                        </Link>
                    </p>
                </form>
            </div>
        </div>
    )
}
