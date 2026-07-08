import { Routes, Route } from 'react-router-dom'
import DashboardLayout from '../layouts/DashboardLayout'
import DashboardHome from '../pages/DashboardHome'
import EmployeeList from '../pages/EmployeeList'
import NotFound from '../pages/NotFound'

export default function AppRouter() {
    return (
        <Routes>
            {/* /dashboard ve altındaki her şey DashboardLayout içinde render edilecek */}
            <Route path="/dashboard" element={<DashboardLayout />}>
                {/* index: path belirtilmeden, tam olarak /dashboard'a gidildiğinde gösterilecek olan */}
                <Route index element={<DashboardHome />} />

                {/* /dashboard/employees */}
                <Route path="employees" element={<EmployeeList />} />
            </Route>

            {/* Hiçbir route'a uymayan URL'ler için */}
            <Route path="*" element={<NotFound />} />
        </Routes>
    )
}