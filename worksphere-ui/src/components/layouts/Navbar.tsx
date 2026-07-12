import { useState, useEffect, useRef } from 'react'
import { Bell, ChevronDown, LogOut, User } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '../../store/hooks'
import { fetchUnreadNotifications, markNotificationAsRead } from '../../store/slices/notificationSlice'
import { logoutUser } from '../../store/slices/authSlice'

export default function Navbar() {
  const dispatch = useAppDispatch()
  const { user } = useAppSelector((state) => state.auth)
  const { items: notifications } = useAppSelector((state) => state.notification)

  const [isProfileOpen, setIsProfileOpen] = useState(false)
  const [isNotificationsOpen, setIsNotificationsOpen] = useState(false)
  const notificationsRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    dispatch(fetchUnreadNotifications())
  }, [dispatch])

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (notificationsRef.current && !notificationsRef.current.contains(event.target as Node)) {
        setIsNotificationsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  const handleNotificationClick = (id: string) => {
    dispatch(markNotificationAsRead(id))
  }

  return (
    <header className="flex h-16 items-center justify-between border-b border-slate-800 bg-slate-900 px-6">
      <div />

      <div className="flex items-center gap-4">
        <div className="relative" ref={notificationsRef}>
          <button
            onClick={() => setIsNotificationsOpen(!isNotificationsOpen)}
            className="relative rounded-full p-2 text-slate-400 hover:bg-slate-800 hover:text-white"
          >
            <Bell size={20} />
            {notifications.length > 0 && (
              <span className="absolute right-1.5 top-1.5 h-2 w-2 rounded-full bg-red-500" />
            )}
          </button>

          {isNotificationsOpen && (
            <div className="absolute right-0 mt-2 w-80 rounded-lg border border-slate-800 bg-slate-900 shadow-lg">
              <div className="border-b border-slate-800 px-4 py-3">
                <span className="text-sm font-medium text-white">Bildirimler</span>
              </div>

              <div className="max-h-80 overflow-y-auto">
                {notifications.length === 0 ? (
                  <p className="px-4 py-6 text-center text-sm text-slate-500">
                    Okunmamış bildirim yok.
                  </p>
                ) : (
                  notifications.map((notification) => (
                    <button
                      key={notification.id}
                      onClick={() => handleNotificationClick(notification.id)}
                      className="block w-full border-b border-slate-800/60 px-4 py-3 text-left last:border-0 hover:bg-slate-800/50"
                    >
                      <p className="text-sm font-medium text-white">{notification.title}</p>
                      <p className="mt-0.5 text-xs text-slate-400">{notification.message}</p>
                    </button>
                  ))
                )}
              </div>
            </div>
          )}
        </div>

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

          {isProfileOpen && (
            <div className="absolute right-0 mt-2 w-48 rounded-lg border border-slate-800 bg-slate-900 py-1 shadow-lg">
              <button className="flex w-full items-center gap-2 px-4 py-2 text-sm text-slate-300 hover:bg-slate-800">
                <User size={16} />
                Profilim
              </button>
              <button
                onClick={() => dispatch(logoutUser())}
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