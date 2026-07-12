// src/types/leaveRequest.ts
export type LeaveType = 'Annual' | 'Sick' | 'Unpaid' | 'Maternity' | 'Other'

export interface CreateLeaveRequestDto {
    leaveType: LeaveType
    startDate: string
    endDate: string
    reason?: string
}

export interface LeaveRequestDto {
    id: string
    employeeId: string
    employeeFullName: string
    leaveType: LeaveType
    startDate: string
    endDate: string
    reason: string | null
    status: 'Pending' | 'Approved' | 'Rejected' | 'Cancelled'
}

export interface LeaveRequestQueryParameters {
    employeeId?: string
    status?: string
    leaveType?: string
    pageNumber?: number
    pageSize?: number
}