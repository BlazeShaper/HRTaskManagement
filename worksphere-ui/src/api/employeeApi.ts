// src/api/employeeApi.ts
import axiosInstance from './axiosInstance'
import type { EmployeeDto, EmployeeQueryParameters } from '../types/employee'
import type { PagedResult } from '../types/common'

export const fetchEmployeesRequest = async (
    params: EmployeeQueryParameters
): Promise<PagedResult<EmployeeDto>> => {
    const response = await axiosInstance.get<PagedResult<EmployeeDto>>('/employee', {
        params,
    })
    return response.data
}

export const deleteEmployeeRequest = async (id: string): Promise<void> => {
    await axiosInstance.delete(`/employee/${id}`)
}