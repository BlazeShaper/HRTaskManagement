// src/types/task.ts
export type TaskStatus = 'Pending' | 'InProgress' | 'Completed' | 'Cancelled'

export interface TaskDto {
    id: string
    title: string
    description: string
    employeeId: string
    employeeFullName: string
    status: TaskStatus
    dueDate: string
}

export interface TaskQueryParameters {
    searchTerm?: string
    employeeId?: string
    status?: TaskStatus
    pageNumber?: number
    pageSize?: number
}