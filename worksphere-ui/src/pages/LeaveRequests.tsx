import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { fetchLeaveRequests, approveLeaveRequest, rejectLeaveRequest } from '../store/slices/leaveRequestSlice'
import { fetchEmployees } from '../store/slices/employeeSlice'
import { Calendar, CheckCircle2, XCircle, FileText, Plus, RefreshCw } from 'lucide-react'
import type { LeaveRequestDto, LeaveType } from '../types/leaveRequest'

const leaveTypeLabels: Record<LeaveType, string> = {
    Annual: 'Yıllık İzin',
    Sick: 'Hastalık İzni',
    Unpaid: 'Ücretsiz İzin',
    Maternity: 'Doğum İzni',
    Other: 'Diğer',
}

const statusColors: Record<LeaveRequestDto['status'], { bg: string; text: string; label: string }> = {
    Pending: { bg: 'bg-slate-800 text-slate-300 border border-slate-700', text: 'text-slate-400', label: 'Beklemede' },
    Approved: { bg: 'bg-emerald-950/80 text-emerald-400 border border-emerald-800', text: 'text-emerald-400', label: 'Onaylandı' },
    Rejected: { bg: 'bg-red-950/80 text-red-400 border border-red-800', text: 'text-red-400', label: 'Reddedildi' },
    Cancelled: { bg: 'bg-zinc-800 text-zinc-400 border border-zinc-700', text: 'text-zinc-500', label: 'İptal Edildi' },
}

export default function LeaveRequests() {
    const navigate = useNavigate()
    const dispatch = useAppDispatch()

    const currentUser = useAppSelector((state) => state.auth.user)
    const { items: leaveRequests, status, actionStatus } = useAppSelector((state) => state.leaveRequest)
    const { items: employees } = useAppSelector((state) => state.employee)

    const [activeTab, setActiveTab] = useState<'my-leaves' | 'approvals'>('my-leaves')
    const [actionId, setActionId] = useState<string | null>(null)

    // Roles checking
    const isManagerOrHR = currentUser?.roles.some((role) => ['Admin', 'Manager', 'HR'].includes(role))

    // Find the current user's employee record
    const currentEmployee = employees.find(
        (emp) => emp.username && currentUser?.username && emp.username.toLowerCase() === currentUser.username.toLowerCase()
    )

    useEffect(() => {
        dispatch(fetchEmployees({ pageSize: 100 }))
    }, [dispatch])

    const loadData = () => {
        if (activeTab === 'my-leaves' && currentEmployee) {
            dispatch(fetchLeaveRequests({ employeeId: currentEmployee.id, pageSize: 50 }))
        } else if (activeTab === 'approvals' && isManagerOrHR) {
            dispatch(fetchLeaveRequests({ status: 'Pending', pageSize: 50 }))
        }
    }

    useEffect(() => {
        loadData()
    }, [dispatch, activeTab, currentEmployee, isManagerOrHR])

    const handleApprove = async (id: string) => {
        setActionId(id)
        await dispatch(approveLeaveRequest(id))
        setActionId(null)
        loadData()
    }

    const handleReject = async (id: string) => {
        setActionId(id)
        await dispatch(rejectLeaveRequest(id))
        setActionId(null)
        loadData()
    }

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('tr-TR', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
        })
    }

    return (
        <div className="space-y-6">
            <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-white tracking-tight">İzin Yönetimi</h1>
                    <p className="mt-1 text-sm text-slate-400">
                        İzin taleplerinizi oluşturun, takip edin veya onay süreçlerini yönetin.
                    </p>
                </div>
                <button
                    onClick={() => navigate('/dashboard/leave-requests/new')}
                    className="flex items-center justify-center gap-2 rounded-lg bg-white px-4 py-2 text-sm font-semibold text-slate-900 transition-all hover:bg-slate-200 hover:scale-[1.02] active:scale-[0.98]"
                >
                    <Plus size={16} />
                    Yeni İzin Talebi
                </button>
            </div>

            {/* Tabs */}
            <div className="flex border-b border-slate-800">
                <button
                    onClick={() => setActiveTab('my-leaves')}
                    className={`border-b-2 px-4 py-2.5 text-sm font-medium transition-all ${
                        activeTab === 'my-leaves'
                            ? 'border-white text-white'
                            : 'border-transparent text-slate-400 hover:text-white'
                    }`}
                >
                    Taleplerim
                </button>
                {isManagerOrHR && (
                    <button
                        onClick={() => setActiveTab('approvals')}
                        className={`relative border-b-2 px-4 py-2.5 text-sm font-medium transition-all ${
                            activeTab === 'approvals'
                                ? 'border-white text-white'
                                : 'border-transparent text-slate-400 hover:text-white'
                        }`}
                    >
                        Onay Bekleyenler
                        {status !== 'loading' && activeTab !== 'approvals' && leaveRequests.some(r => r.status === 'Pending') && (
                            <span className="absolute right-0.5 top-2.5 h-2 w-2 rounded-full bg-indigo-500 animate-pulse" />
                        )}
                    </button>
                )}
            </div>

            {/* List */}
            {status === 'loading' ? (
                <div className="flex h-48 items-center justify-center rounded-xl border border-slate-800 bg-slate-900/50 backdrop-blur-md">
                    <div className="flex flex-col items-center gap-2 text-slate-400">
                        <RefreshCw className="h-6 w-6 animate-spin text-indigo-400" />
                        <span className="text-sm">İzin talepleri yükleniyor...</span>
                    </div>
                </div>
            ) : leaveRequests.length === 0 ? (
                <div className="flex flex-col items-center justify-center rounded-xl border border-slate-800 bg-slate-900/30 p-12 text-center">
                    <FileText className="mx-auto h-12 w-12 text-slate-600" />
                    <h3 className="mt-4 text-lg font-medium text-white">Talep Bulunamadı</h3>
                    <p className="mt-1 text-sm text-slate-500">
                        {activeTab === 'my-leaves'
                            ? 'Henüz bir izin talebi oluşturmadınız.'
                            : 'Onayınızı bekleyen herhangi bir izin talebi bulunmuyor.'}
                    </p>
                </div>
            ) : (
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                    {leaveRequests.map((request) => {
                        const leaveColor = statusColors[request.status]
                        const isActionPending = actionId === request.id && actionStatus === 'loading'

                        return (
                            <div
                                key={request.id}
                                className="group relative overflow-hidden rounded-xl border border-slate-800 bg-slate-900/60 p-5 backdrop-blur-sm transition-all duration-300 hover:border-slate-700 hover:shadow-lg hover:shadow-slate-950/20"
                            >
                                <div className="flex items-start justify-between">
                                    <div className="space-y-1">
                                        <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold ${leaveColor.bg}`}>
                                            {leaveColor.label}
                                        </span>
                                        <h3 className="text-base font-bold text-white mt-2">
                                            {leaveTypeLabels[request.leaveType]}
                                        </h3>
                                        {activeTab === 'approvals' && (
                                            <p className="text-xs text-slate-400 font-medium">
                                                Talep Eden: <span className="text-white">{request.employeeFullName}</span>
                                            </p>
                                        )}
                                    </div>
                                    <div className="flex items-center text-slate-500 gap-1 text-xs">
                                        <Calendar size={14} className="text-slate-400" />
                                        <span>
                                            {Math.round(
                                                (new Date(request.endDate).getTime() - new Date(request.startDate).getTime()) /
                                                    (1000 * 3600 * 24)
                                            ) + 1}{' '}
                                            Gün
                                        </span>
                                    </div>
                                </div>

                                <div className="mt-4 space-y-2 text-sm border-t border-slate-800/60 pt-4">
                                    <div className="flex justify-between text-xs text-slate-400">
                                        <span>Başlangıç:</span>
                                        <span className="text-white font-medium">{formatDate(request.startDate)}</span>
                                    </div>
                                    <div className="flex justify-between text-xs text-slate-400">
                                        <span>Bitiş:</span>
                                        <span className="text-white font-medium">{formatDate(request.endDate)}</span>
                                    </div>
                                    {request.reason && (
                                        <div className="mt-2 rounded-lg bg-slate-950/40 p-2 text-xs text-slate-300 border border-slate-800/30">
                                            <p className="italic">"{request.reason}"</p>
                                        </div>
                                    )}
                                </div>

                                {activeTab === 'approvals' && request.status === 'Pending' && (
                                    <div className="mt-5 flex gap-2 border-t border-slate-800/60 pt-4">
                                        <button
                                            onClick={() => handleReject(request.id)}
                                            disabled={isActionPending}
                                            className="flex flex-1 items-center justify-center gap-1.5 rounded-lg border border-red-900/60 bg-red-950/20 py-2 text-xs font-semibold text-red-400 transition-colors hover:bg-red-900/20 active:scale-95 disabled:pointer-events-none disabled:opacity-50"
                                        >
                                            <XCircle size={14} />
                                            Reddet
                                        </button>
                                        <button
                                            onClick={() => handleApprove(request.id)}
                                            disabled={isActionPending}
                                            className="flex flex-1 items-center justify-center gap-1.5 rounded-lg border border-emerald-950 bg-emerald-950 py-2 text-xs font-semibold text-emerald-400 transition-colors hover:bg-emerald-900/40 active:scale-95 disabled:pointer-events-none disabled:opacity-50"
                                        >
                                            <CheckCircle2 size={14} />
                                            Onayla
                                        </button>
                                    </div>
                                )}
                            </div>
                        )
                    })}
                </div>
            )}
        </div>
    )
}
