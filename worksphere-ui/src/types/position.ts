// src/types/position.ts
export interface PositionDto {
  id: string
  title: string
  description: string | null
  employeeCount: number
}

export interface CreatePositionDto {
  title: string
  description?: string
}

export interface PositionQueryParameters {
  searchTerm?: string
  pageNumber?: number
  pageSize?: number
}
