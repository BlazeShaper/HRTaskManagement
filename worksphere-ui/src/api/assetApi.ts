// src/api/assetApi.ts
import axiosInstance from './axiosInstance'
import type { AssetDto, AssetQueryParameters } from '../types/asset'
import type { PagedResult } from '../types/common'

export const fetchAssetsRequest = async (
    params: AssetQueryParameters
): Promise<PagedResult<AssetDto>> => {
    const response = await axiosInstance.get<PagedResult<AssetDto>>('/asset', { params })
    return response.data
}