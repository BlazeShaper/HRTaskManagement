// src/api/departmentApi.ts
import axiosInstance from './axiosInstance'
import type { DepartmentDto, CreateDepartmentDto, DepartmentQueryParameters } from '../types/department'
import type { PagedResult } from '../types/common'

export const fetchDepartmentsRequest = async (
  params: DepartmentQueryParameters
): Promise<PagedResult<DepartmentDto>> => {
  const response = await axiosInstance.get<PagedResult<DepartmentDto>>('/department', { params })
  return response.data
}

export const createDepartmentRequest = async (dto: CreateDepartmentDto): Promise<DepartmentDto> => {
  const response = await axiosInstance.post<DepartmentDto>('/department', dto)
  return response.data
}
