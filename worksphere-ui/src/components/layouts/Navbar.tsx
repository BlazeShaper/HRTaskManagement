import { useState } from 'react'
import { Bell, ChevronDown, LogOut, User } from 'lucide-react'
import { useAppSelector, useAppDispatch } from '../../store/hooks'
import { logout } from '../../store/slices/authSlice'

export default function Navbar() {
    const user = useAppSelector((state) => state.auth.user)
    const dispatch = useAppDispatch()
    // useState: React'te "değişebilen veri" (state) tutmanın yolu.
    // isProfileOpen: dropdown açık mı kapalı mı bilgisini tutuyor.
    // setIsProfileOpen: bu değeri değiştirmek için kullandığımız fonksiyon.
    const [isProfileOpen, setIsProfileOpen] = useState(false)

    return (
        <header className="flex h-16 items-center justify-between border-b border-slate-800 bg-slate-900 px-6">
            {/* Sol taraf: şimdilik boş bırakıyoruz, ileride breadcrumb/sayfa başlığı gelebilir */}
            <div />

            {/* Sağ taraf: bildirim + profil */}
            <div className="flex items-center gap-4">
                {/* Bildirim ikonu */}
                <button className="relative rounded-full p-2 text-slate-400 hover:bg-slate-800 hover:text-white">
                    <Bell size={20} />
                    {/* Okunmamış bildirim varsa gösterilecek kırmızı nokta */}
                    <span className="absolute right-1.5 top-1.5 h-2 w-2 rounded-full bg-red-500" />
                </button>

                {/* Profil alanı */}
                <div className="relative">
                    <button
                        onClick={() => setIsProfileOpen(!isProfileOpen)}
                        className="flex items-center gap-2 rounded-lg px-2 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
                    >
                        <div className="flex h-8 w-8 items-center justify-center rounded-full bg-slate-700">
                            <User size={16} />
                        </div>
                        <span className="font-medium">{user?.username ?? 'Misafir'}</span>
                        <ChevronDown size={16} />
                    </button>

                    {/* Dropdown menü — sadece isProfileOpen true ise gösteriliyor */}
                    {isProfileOpen && (
                        <div className="absolute right-0 mt-2 w-48 rounded-lg border border-slate-800 bg-slate-900 py-1 shadow-lg">
                            <button className="flex w-full items-center gap-2 px-4 py-2 text-sm text-slate-300 hover:bg-slate-800">
                                <User size={16} />
                                Profilim
                            </button>
                            <button
                                onClick={() => dispatch(logout())}
                                className="flex w-full items-center gap-2 px-4 py-2 text-sm text-red-400 hover:bg-slate-800"
                            >
                                <LogOut size={16} />
                                Çıkış Yap
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </header>
    )
}