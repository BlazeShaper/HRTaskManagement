// src/router/index.tsx
import { Routes, Route } from 'react-router-dom'
import DashboardLayout from '../layouts/DashboardLayout'
import DashboardHome from '../pages/DashboardHome'
import EmployeeList from '../pages/EmployeeList'
import EmployeeCreate from '../pages/EmployeeCreate'
import TaskList from '../pages/TaskList'
import Login from '../pages/Login'
import Register from '../pages/Register'
import AccessDenied from '../pages/AccessDenied'
import NotFound from '../pages/NotFound'
import ProtectedRoute from '../components/routing/ProtectedRoute'

export default function AppRouter() {
    return (
        <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/access-denied" element={<AccessDenied />} />

            <Route
                path="/dashboard"
                element={
                    <ProtectedRoute>
                        <DashboardLayout />
                    </ProtectedRoute>
                }
            >
                <Route index element={<DashboardHome />} />
                <Route path="employees" element={<EmployeeList />} />
                <Route
                    path="employees/new"
                    element={
                        <ProtectedRoute roles={['Admin', 'HR']}>
                            <EmployeeCreate />
                        </ProtectedRoute>
                    }
                />
                <Route path="tasks" element={<TaskList />} />
            </Route>

            <Route path="*" element={<NotFound />} />
        </Routes>
    )
}