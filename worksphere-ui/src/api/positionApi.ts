// src/api/positionApi.ts
import axiosInstance from './axiosInstance'
import type { PositionDto, CreatePositionDto, PositionQueryParameters } from '../types/position'
import type { PagedResult } from '../types/common'

export const fetchPositionsRequest = async (
  params: PositionQueryParameters
): Promise<PagedResult<PositionDto>> => {
  const response = await axiosInstance.get<PagedResult<PositionDto>>('/position', { params })
  return response.data
}

export const createPositionRequest = async (dto: CreatePositionDto): Promise<PositionDto> => {
  const response = await axiosInstance.post<PositionDto>('/position', dto)
  return response.data
}
