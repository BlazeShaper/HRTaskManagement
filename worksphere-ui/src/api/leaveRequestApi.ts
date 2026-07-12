// src/api/leaveRequestApi.ts
import axiosInstance from './axiosInstance'
import type { CreateLeaveRequestDto, LeaveRequestDto, LeaveRequestQueryParameters } from '../types/leaveRequest'
import type { PagedResult } from '../types/common'

export const createLeaveRequestRequest = async (
    dto: CreateLeaveRequestDto
): Promise<LeaveRequestDto> => {
    const response = await axiosInstance.post<LeaveRequestDto>('/leaverequest', dto)
    return response.data
}

export const getLeaveRequestsRequest = async (
    params?: LeaveRequestQueryParameters
): Promise<PagedResult<LeaveRequestDto>> => {
    const response = await axiosInstance.get<PagedResult<LeaveRequestDto>>('/leaverequest', { params })
    return response.data
}

export const approveLeaveRequestRequest = async (id: string): Promise<LeaveRequestDto> => {
    const response = await axiosInstance.patch<LeaveRequestDto>(`/leaverequest/${id}/approve`)
    return response.data
}

export const rejectLeaveRequestRequest = async (id: string): Promise<LeaveRequestDto> => {
    const response = await axiosInstance.patch<LeaveRequestDto>(`/leaverequest/${id}/reject`)
    return response.data
}