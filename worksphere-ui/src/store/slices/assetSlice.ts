// src/store/slices/assetSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchAssetsRequest } from '../../api/assetApi'
import type { AssetDto, AssetQueryParameters } from '../../types/asset'

interface AssetState {
    items: AssetDto[]
    totalCount: number
    pageNumber: number
    pageSize: number
    status: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

const initialState: AssetState = {
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    status: 'idle',
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

const assetSlice = createSlice({
    name: 'asset',
    initialState,
    reducers: {},
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
    },
})

export default assetSlice.reducer