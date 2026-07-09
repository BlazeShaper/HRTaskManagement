// src/store/slices/employeeSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchEmployeesRequest, deleteEmployeeRequest } from '../../api/employeeApi'
import type { EmployeeDto, EmployeeQueryParameters } from '../../types/employee'

interface EmployeeState {
    items: EmployeeDto[]
    totalCount: number
    pageNumber: number
    pageSize: number
    status: 'idle' | 'loading' | 'succeeded' | 'failed'
    deleteStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

const initialState: EmployeeState = {
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    status: 'idle',
    deleteStatus: 'idle',
    error: null,
}

export const fetchEmployees = createAsyncThunk(
    'employee/fetchEmployees',
    async (params: EmployeeQueryParameters, { rejectWithValue }) => {
        try {
            return await fetchEmployeesRequest(params)
        } catch {
            return rejectWithValue('Çalışanlar yüklenirken bir hata oluştu.')
        }
    }
)

export const deleteEmployee = createAsyncThunk(
    'employee/deleteEmployee',
    async (id: string, { rejectWithValue }) => {
        try {
            await deleteEmployeeRequest(id)
            return id
        } catch {
            return rejectWithValue('Çalışan silinirken bir hata oluştu.')
        }
    }
)

const employeeSlice = createSlice({
    name: 'employee',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchEmployees.pending, (state) => {
                state.status = 'loading'
                state.error = null
            })
            .addCase(fetchEmployees.fulfilled, (state, action) => {
                state.status = 'succeeded'
                state.items = action.payload.items
                state.totalCount = action.payload.totalCount
                state.pageNumber = action.payload.pageNumber
                state.pageSize = action.payload.pageSize
            })
            .addCase(fetchEmployees.rejected, (state, action) => {
                state.status = 'failed'
                state.error = action.payload as string
            })
            .addCase(deleteEmployee.pending, (state) => {
                state.deleteStatus = 'loading'
            })
            .addCase(deleteEmployee.fulfilled, (state, action) => {
                state.deleteStatus = 'succeeded'
                state.items = state.items.filter((e) => e.id !== action.payload)
            })
            .addCase(deleteEmployee.rejected, (state, action) => {
                state.deleteStatus = 'failed'
                state.error = action.payload as string
            })
    },
})

export default employeeSlice.reducer