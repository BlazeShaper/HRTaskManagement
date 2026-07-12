// src/store/slices/leaveRequestSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import {
    createLeaveRequestRequest,
    getLeaveRequestsRequest,
    approveLeaveRequestRequest,
    rejectLeaveRequestRequest
} from '../../api/leaveRequestApi'
import type { CreateLeaveRequestDto, LeaveRequestDto, LeaveRequestQueryParameters } from '../../types/leaveRequest'

interface LeaveRequestState {
    items: LeaveRequestDto[]
    totalCount: number
    pageNumber: number
    pageSize: number
    status: 'idle' | 'loading' | 'succeeded' | 'failed'
    createStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
    actionStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

const initialState: LeaveRequestState = {
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    status: 'idle',
    createStatus: 'idle',
    actionStatus: 'idle',
    error: null,
}

export const fetchLeaveRequests = createAsyncThunk(
    'leaveRequest/fetchAll',
    async (params: LeaveRequestQueryParameters, { rejectWithValue }) => {
        try {
            return await getLeaveRequestsRequest(params)
        } catch (err: any) {
            return rejectWithValue(
                err.response?.data?.message ||
                err.response?.data?.Message ||
                'İzin talepleri yüklenirken bir hata oluştu.'
            )
        }
    }
)

export const createLeaveRequest = createAsyncThunk(
    'leaveRequest/create',
    async (dto: CreateLeaveRequestDto, { rejectWithValue }) => {
        try {
            return await createLeaveRequestRequest(dto)
        } catch (err: any) {
            return rejectWithValue(
                err.response?.data?.message ||
                err.response?.data?.Message ||
                'İzin talebi oluşturulurken bir hata oluştu.'
            )
        }
    }
)

export const approveLeaveRequest = createAsyncThunk(
    'leaveRequest/approve',
    async (id: string, { rejectWithValue }) => {
        try {
            return await approveLeaveRequestRequest(id)
        } catch (err: any) {
            return rejectWithValue(
                err.response?.data?.message ||
                err.response?.data?.Message ||
                'İzin talebi onaylanırken bir hata oluştu.'
            )
        }
    }
)

export const rejectLeaveRequest = createAsyncThunk(
    'leaveRequest/reject',
    async (id: string, { rejectWithValue }) => {
        try {
            return await rejectLeaveRequestRequest(id)
        } catch (err: any) {
            return rejectWithValue(
                err.response?.data?.message ||
                err.response?.data?.Message ||
                'İzin talebi reddedilirken bir hata oluştu.'
            )
        }
    }
)

const leaveRequestSlice = createSlice({
    name: 'leaveRequest',
    initialState,
    reducers: {
        resetCreateStatus: (state) => {
            state.createStatus = 'idle'
            state.error = null
        },
    },
    extraReducers: (builder) => {
        builder
            // Fetch List
            .addCase(fetchLeaveRequests.pending, (state) => {
                state.status = 'loading'
                state.error = null
            })
            .addCase(fetchLeaveRequests.fulfilled, (state, action) => {
                state.status = 'succeeded'
                state.items = action.payload.items
                state.totalCount = action.payload.totalCount
                state.pageNumber = action.payload.pageNumber
                state.pageSize = action.payload.pageSize
            })
            .addCase(fetchLeaveRequests.rejected, (state, action) => {
                state.status = 'failed'
                state.error = action.payload as string
            })
            // Create
            .addCase(createLeaveRequest.pending, (state) => {
                state.createStatus = 'loading'
                state.error = null
            })
            .addCase(createLeaveRequest.fulfilled, (state, action) => {
                state.createStatus = 'succeeded'
                state.items = [action.payload, ...state.items]
            })
            .addCase(createLeaveRequest.rejected, (state, action) => {
                state.createStatus = 'failed'
                state.error = action.payload as string
            })
            // Approve
            .addCase(approveLeaveRequest.pending, (state) => {
                state.actionStatus = 'loading'
                state.error = null
            })
            .addCase(approveLeaveRequest.fulfilled, (state, action) => {
                state.actionStatus = 'succeeded'
                const index = state.items.findIndex(item => item.id === action.payload.id)
                if (index !== -1) {
                    state.items[index] = action.payload
                }
            })
            .addCase(approveLeaveRequest.rejected, (state, action) => {
                state.actionStatus = 'failed'
                state.error = action.payload as string
            })
            // Reject
            .addCase(rejectLeaveRequest.pending, (state) => {
                state.actionStatus = 'loading'
                state.error = null
            })
            .addCase(rejectLeaveRequest.fulfilled, (state, action) => {
                state.actionStatus = 'succeeded'
                const index = state.items.findIndex(item => item.id === action.payload.id)
                if (index !== -1) {
                    state.items[index] = action.payload
                }
            })
            .addCase(rejectLeaveRequest.rejected, (state, action) => {
                state.actionStatus = 'failed'
                state.error = action.payload as string
            })
    },
})

export const { resetCreateStatus } = leaveRequestSlice.actions
export default leaveRequestSlice.reducer