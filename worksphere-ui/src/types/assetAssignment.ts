// src/types/assetAssignment.ts
export interface AssetAssignmentDto {
    id: string
    assetId: string
    assetName: string
    assetSerialNumber: string
    employeeId: string
    employeeFullName: string
    assignedByUserId: string
    assignedByUsername: string
    assignedDate: string
    returnedDate: string | null
    note: string | null
    isActive: boolean
}

export interface CreateAssetAssignmentDto {
    assetId: string
    employeeId: string
    note?: string
}

export interface ReturnAssetDto {
    returnNote?: string
}