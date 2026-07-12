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
import LeaveRequestCreate from '../pages/LeaveRequestCreate'
import LeaveRequests from '../pages/LeaveRequests'
import AssetList from '../pages/AssetList'

export default function AppRouter() {
    return (
        <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/access-denied" element={<AccessDenied />} />
            <Route path="assets" element={<AssetList />} />

            <Route
                path="/dashboard"
                element={
                    <ProtectedRoute>
                        <DashboardLayout />
                    </ProtectedRoute>
                }
            >
                <Route
                    path="leave-requests"
                    element={
                        <ProtectedRoute>
                            <LeaveRequests />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="leave-requests/new"
                    element={
                        <ProtectedRoute>
                            <LeaveRequestCreate />
                        </ProtectedRoute>
                    }
                />

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