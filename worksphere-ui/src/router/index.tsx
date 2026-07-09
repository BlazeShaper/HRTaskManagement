// src/router/index.tsx
import { Routes, Route } from 'react-router-dom'
import DashboardLayout from '../layouts/DashboardLayout'
import DashboardHome from '../pages/DashboardHome'
import EmployeeList from '../pages/EmployeeList'
import Login from '../pages/Login'
import NotFound from '../pages/NotFound'

export default function AppRouter() {
    return (
        <Routes>
            <Route path="/login" element={<Login />} />

            <Route path="/dashboard" element={<DashboardLayout />}>
                <Route index element={<DashboardHome />} />
                <Route path="employees" element={<EmployeeList />} />
            </Route>

            <Route path="*" element={<NotFound />} />
        </Routes>
    )
}