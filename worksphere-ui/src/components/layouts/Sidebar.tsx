import { NavLink } from 'react-router-dom'
import {
    LayoutDashboard,
    Users,
    Building2,
    Briefcase,
    ListTodo,
    Package,
    CalendarClock,
} from 'lucide-react'

// Bu dizi, sidebar'daki her linki tek bir yerden yönetmemizi sağlıyor.
// Yeni bir sayfa eklediğinizde tek yapmanız gereken buraya bir satır eklemek.
const navItems = [
    { to: '/dashboard', label: 'Panel', icon: LayoutDashboard },
    { to: '/dashboard/employees', label: 'Çalışanlar', icon: Users },
    { to: '/dashboard/departments', label: 'Departmanlar', icon: Building2 },
    { to: '/dashboard/positions', label: 'Pozisyonlar', icon: Briefcase },
    { to: '/dashboard/tasks', label: 'Görevler', icon: ListTodo },
    { to: '/dashboard/assets', label: 'Demirbaşlar', icon: Package },
    { to: '/dashboard/leave-requests', label: 'İzin Talepleri', icon: CalendarClock },
]

export default function Sidebar() {
    return (
        <aside className="flex h-screen w-64 flex-col border-r border-slate-800 bg-slate-900">
            {/* Logo / Marka alanı */}
            <div className="flex h-16 items-center border-b border-slate-800 px-6">
                <span className="text-lg font-bold text-white">WorkSphere</span>
            </div>

            {/* Navigasyon linkleri */}
            <nav className="flex-1 space-y-1 overflow-y-auto px-3 py-4">
                {navItems.map((item) => {
                    const Icon = item.icon
                    return (
                        <NavLink
                            key={item.to}
                            to={item.to}
                            end={item.to === '/dashboard'}
                            className={({ isActive }) =>
                                `flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors ${isActive
                                    ? 'bg-slate-800 text-white'
                                    : 'text-slate-400 hover:bg-slate-800/50 hover:text-white'
                                }`
                            }
                        >
                            <Icon size={18} />
                            {item.label}
                        </NavLink>
                    )
                })}
            </nav>
        </aside>
    )
}