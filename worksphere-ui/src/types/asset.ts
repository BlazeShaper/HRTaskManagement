// src/types/asset.ts
export type AssetStatus = 'Available' | 'Assigned' | 'UnderRepair' | 'Retired'

export interface AssetDto {
  id: string
  name: string
  assetType: string
  serialNumber: string
  purchaseDate: string
  status: AssetStatus
}

export interface AssetQueryParameters {
  searchTerm?: string
  assetType?: string
  status?: AssetStatus
  pageNumber?: number
  pageSize?: number
}