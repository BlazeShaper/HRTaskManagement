// src/types/department.ts
export interface DepartmentDto {
  id: string
  name: string
  description: string | null
  managerId: string | null
  managerFullName: string | null
  employeeCount: number
}

export interface CreateDepartmentDto {
  name: string
  description?: string
  managerId?: string
}

export interface DepartmentQueryParameters {
  searchTerm?: string
  pageNumber?: number
  pageSize?: number
}
