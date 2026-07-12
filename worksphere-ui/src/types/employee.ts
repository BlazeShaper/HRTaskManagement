// src/types/employee.ts
export interface EmployeeDto {
    id: string
    firstName: string
    lastName: string
    email: string
    phone: string
    birthDate: string
    hireDate: string
    isActive: boolean
    departmentId: string
    departmentName: string
    positionId: string
    positionTitle: string
    username: string
}

export interface EmployeeQueryParameters {
    searchTerm?: string
    departmentId?: string
    positionId?: string
    isActive?: boolean
    pageNumber?: number
    pageSize?: number
}