// src/api/assetApi.ts
import axiosInstance from './axiosInstance'
import type { AssetDto, CreateAssetDto, AssetQueryParameters } from '../types/asset'
import type { PagedResult } from '../types/common'

export const fetchAssetsRequest = async (
  params: AssetQueryParameters
): Promise<PagedResult<AssetDto>> => {
  const response = await axiosInstance.get<PagedResult<AssetDto>>('/asset', { params })
  return response.data
}

export const createAssetRequest = async (dto: CreateAssetDto): Promise<AssetDto> => {
  const response = await axiosInstance.post<AssetDto>('/asset', dto)
  return response.data
}