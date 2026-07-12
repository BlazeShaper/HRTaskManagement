// src/store/slices/assetAssignmentSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { assignAssetRequest, returnAssetRequest, fetchActiveAssignmentRequest } from '../../api/assetAssignmentApi'
import type { CreateAssetAssignmentDto, ReturnAssetDto } from '../../types/assetAssignment'

interface AssetAssignmentState {
    assignStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
    returnStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

const initialState: AssetAssignmentState = {
    assignStatus: 'idle',
    returnStatus: 'idle',
    error: null,
}

export const assignAsset = createAsyncThunk(
    'assetAssignment/assign',
    async (dto: CreateAssetAssignmentDto, { rejectWithValue }) => {
        try {
            return await assignAssetRequest(dto)
        } catch (error: any) {
            return rejectWithValue(
                error.response?.data?.message || 'Demirbaş zimmetlenirken bir hata oluştu.'
            )
        }
    }
)

export const returnAsset = createAsyncThunk(
    'assetAssignment/return',
    async ({ assignmentId, dto }: { assignmentId: string; dto: ReturnAssetDto }, { rejectWithValue }) => {
        try {
            return await returnAssetRequest(assignmentId, dto)
        } catch (error: any) {
            return rejectWithValue(
                error.response?.data?.message || 'Demirbaş iade edilirken bir hata oluştu.'
            )
        }
    }
)

export const fetchActiveAssignmentByAssetId = createAsyncThunk(
    'assetAssignment/fetchActiveByAssetId',
    async (assetId: string, { rejectWithValue }) => {
        try {
            const result = await fetchActiveAssignmentRequest(assetId)
            const activeAssignment = result.items[0]
            if (!activeAssignment) {
                return rejectWithValue('Aktif zimmet kaydı bulunamadı.')
            }
            return activeAssignment
        } catch (error: any) {
            return rejectWithValue(
                error.response?.data?.message || 'Aktif zimmet kaydı sorgulanırken bir hata oluştu.'
            )
        }
    }
)

const assetAssignmentSlice = createSlice({
    name: 'assetAssignment',
    initialState,
    reducers: {
        resetAssignStatus: (state) => {
            state.assignStatus = 'idle'
            state.error = null
        },
        resetReturnStatus: (state) => {
            state.returnStatus = 'idle'
            state.error = null
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(assignAsset.pending, (state) => {
                state.assignStatus = 'loading'
                state.error = null
            })
            .addCase(assignAsset.fulfilled, (state) => {
                state.assignStatus = 'succeeded'
            })
            .addCase(assignAsset.rejected, (state, action) => {
                state.assignStatus = 'failed'
                state.error = action.payload as string
            })
            .addCase(returnAsset.pending, (state) => {
                state.returnStatus = 'loading'
                state.error = null
            })
            .addCase(returnAsset.fulfilled, (state) => {
                state.returnStatus = 'succeeded'
            })
            .addCase(returnAsset.rejected, (state, action) => {
                state.returnStatus = 'failed'
                state.error = action.payload as string
            })
    },
})

export const { resetAssignStatus, resetReturnStatus } = assetAssignmentSlice.actions
export default assetAssignmentSlice.reducer