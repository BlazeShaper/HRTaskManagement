// src/api/assetAssignmentApi.ts
import axiosInstance from './axiosInstance'
import type { AssetAssignmentDto, CreateAssetAssignmentDto, ReturnAssetDto } from '../types/assetAssignment'
import type { PagedResult } from '../types/common'

export const assignAssetRequest = async (
    dto: CreateAssetAssignmentDto
): Promise<AssetAssignmentDto> => {
    const response = await axiosInstance.post<AssetAssignmentDto>('/assetassignment', dto)
    return response.data
}

export const returnAssetRequest = async (
    assignmentId: string,
    dto: ReturnAssetDto
): Promise<AssetAssignmentDto> => {
    const response = await axiosInstance.patch<AssetAssignmentDto>(`/assetassignment/${assignmentId}/return`, dto)
    return response.data
}

export const fetchActiveAssignmentRequest = async (
    assetId: string
): Promise<PagedResult<AssetAssignmentDto>> => {
    const response = await axiosInstance.get<PagedResult<AssetAssignmentDto>>('/assetassignment', {
        params: { assetId, onlyActive: true }
    })
    return response.data
}