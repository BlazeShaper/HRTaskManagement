// src/pages/LeaveRequestCreate.tsx
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../store/hooks'
import { createLeaveRequest, resetCreateStatus } from '../store/slices/leaveRequestSlice'
import Select from '../components/ui/Select'
import type { LeaveType } from '../types/leaveRequest'

const leaveTypeOptions: { value: LeaveType; label: string }[] = [
    { value: 'Annual', label: 'Yıllık İzin' },
    { value: 'Sick', label: 'Hastalık İzni' },
    { value: 'Unpaid', label: 'Ücretsiz İzin' },
    { value: 'Maternity', label: 'Doğum İzni' },
    { value: 'Other', label: 'Diğer' },
]

export default function LeaveRequestCreate() {
    const navigate = useNavigate()
    const dispatch = useAppDispatch()
    const { createStatus, error } = useAppSelector((state) => state.leaveRequest)

    const [leaveType, setLeaveType] = useState<LeaveType>('Annual')
    const [startDate, setStartDate] = useState('')
    const [endDate, setEndDate] = useState('')
    const [reason, setReason] = useState('')

    const isLoading = createStatus === 'loading'
    const dateError = startDate && endDate && endDate < startDate
        ? 'Bitiş tarihi başlangıç tarihinden önce olamaz.'
        : null

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault()
        if (dateError) return

        const result = await dispatch(
            createLeaveRequest({
                leaveType,
                startDate,
                endDate,
                reason: reason.trim() || undefined,
            })
        )

        if (createLeaveRequest.fulfilled.match(result)) {
            dispatch(resetCreateStatus())
            navigate('/dashboard/leave-requests')
        }
    }

    return (
        <div className="mx-auto max-w-lg">
            <h1 className="text-2xl font-bold text-white">İzin Talebi Oluştur</h1>

            <form
                onSubmit={handleSubmit}
                className="mt-6 rounded-xl border border-slate-800 bg-slate-900 p-6"
            >
                <div className="mb-4">
                    <label className="mb-1.5 block text-sm font-medium text-slate-300">
                        İzin Türü
                    </label>
                    <Select
                        value={leaveType}
                        onChange={(value) => setLeaveType(value as LeaveType)}
                        options={leaveTypeOptions}
                    />
                </div>

                <div className="mb-4 grid grid-cols-2 gap-4">
                    <div>
                        <label htmlFor="startDate" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Başlangıç Tarihi
                        </label>
                        <input
                            id="startDate"
                            type="date"
                            value={startDate}
                            onChange={(e) => setStartDate(e.target.value)}
                            required
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
                        />
                    </div>

                    <div>
                        <label htmlFor="endDate" className="mb-1.5 block text-sm font-medium text-slate-300">
                            Bitiş Tarihi
                        </label>
                        <input
                            id="endDate"
                            type="date"
                            value={endDate}
                            onChange={(e) => setEndDate(e.target.value)}
                            required
                            className="w-full rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
                        />
                    </div>
                </div>

                {dateError && (
                    <p className="mb-4 text-sm text-red-400">{dateError}</p>
                )}

                <div className="mb-5">
                    <label htmlFor="reason" className="mb-1.5 block text-sm font-medium text-slate-300">
                        Açıklama <span className="text-slate-500">(opsiyonel)</span>
                    </label>
                    <textarea
                        id="reason"
                        value={reason}
                        onChange={(e) => setReason(e.target.value)}
                        rows={3}
                        className="w-full resize-none rounded-lg border border-slate-700 bg-slate-800 px-3 py-2 text-sm text-white outline-none focus:border-slate-500"
                        placeholder="İzin talebinizle ilgili eklemek istediğiniz bir not varsa yazabilirsiniz."
                    />
                </div>

                {error && (
                    <div className="mb-4 rounded-lg border border-red-900 bg-red-950/50 px-3 py-2 text-sm text-red-400">
                        {error}
                    </div>
                )}

                <button
                    type="submit"
                    disabled={isLoading || !!dateError}
                    className="w-full rounded-lg bg-white py-2 text-sm font-medium text-slate-900 hover:bg-slate-200 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    {isLoading ? 'Gönderiliyor...' : 'Talebi Gönder'}
                </button>
            </form>
        </div>
    )
}