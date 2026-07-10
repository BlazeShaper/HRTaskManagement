// src/components/routing/ProtectedRoute.tsx
import { Navigate } from 'react-router-dom'
import { useAppSelector } from '../../store/hooks'

interface ProtectedRouteProps {
    children: React.ReactNode
    roles?: string[]
}

export default function ProtectedRoute({ children, roles }: ProtectedRouteProps) {
    const { user, accessToken } = useAppSelector((state) => state.auth)

    if (!accessToken) {
        return <Navigate to="/login" replace />
    }

    if (roles && roles.length > 0) {
        const hasRequiredRole = user?.roles.some((role) => roles.includes(role))

        if (!hasRequiredRole) {
            return <Navigate to="/access-denied" replace />
        }
    }

    return <>{children}</>
}