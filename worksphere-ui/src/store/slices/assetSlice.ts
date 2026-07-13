// src/store/slices/assetSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchAssetsRequest, createAssetRequest } from '../../api/assetApi'
import type { AssetDto, CreateAssetDto, AssetQueryParameters } from '../../types/asset'

interface AssetState {
  items: AssetDto[]
  totalCount: number
  pageNumber: number
  pageSize: number
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  createStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: AssetState = {
  items: [],
  totalCount: 0,
  pageNumber: 1,
  pageSize: 10,
  status: 'idle',
  createStatus: 'idle',
  error: null,
}

export const fetchAssets = createAsyncThunk(
  'asset/fetchAssets',
  async (params: AssetQueryParameters, { rejectWithValue }) => {
    try {
      return await fetchAssetsRequest(params)
    } catch {
      return rejectWithValue('Demirbaşlar yüklenirken bir hata oluştu.')
    }
  }
)

export const createAsset = createAsyncThunk(
  'asset/createAsset',
  async (dto: CreateAssetDto, { rejectWithValue }) => {
    try {
      return await createAssetRequest(dto)
    } catch (error) {
      const err = error as { response?: { data?: { message?: string } } }
      return rejectWithValue(
        err.response?.data?.message || 'Demirbaş oluşturulurken bir hata oluştu.'
      )
    }
  }
)

const assetSlice = createSlice({
  name: 'asset',
  initialState,
  reducers: {
    resetCreateStatus: (state) => {
      state.createStatus = 'idle'
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAssets.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(fetchAssets.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.items = action.payload.items
        state.totalCount = action.payload.totalCount
        state.pageNumber = action.payload.pageNumber
        state.pageSize = action.payload.pageSize
      })
      .addCase(fetchAssets.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
      .addCase(createAsset.pending, (state) => {
        state.createStatus = 'loading'
        state.error = null
      })
      .addCase(createAsset.fulfilled, (state) => {
        state.createStatus = 'succeeded'
      })
      .addCase(createAsset.rejected, (state, action) => {
        state.createStatus = 'failed'
        state.error = action.payload as string
      })
  },
})

export const { resetCreateStatus } = assetSlice.actions
export default assetSlice.reducer