// src/store/slices/departmentSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchDepartmentsRequest, createDepartmentRequest } from '../../api/departmentApi'
import type { DepartmentDto, CreateDepartmentDto, DepartmentQueryParameters } from '../../types/department'

interface DepartmentState {
  items: DepartmentDto[]
  totalCount: number
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  createStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: DepartmentState = {
  items: [],
  totalCount: 0,
  status: 'idle',
  createStatus: 'idle',
  error: null,
}

export const fetchDepartments = createAsyncThunk(
  'department/fetchDepartments',
  async (params: DepartmentQueryParameters, { rejectWithValue }) => {
    try {
      return await fetchDepartmentsRequest(params)
    } catch {
      return rejectWithValue('Departmanlar yüklenirken bir hata oluştu.')
    }
  }
)

export const createDepartment = createAsyncThunk(
  'department/createDepartment',
  async (dto: CreateDepartmentDto, { rejectWithValue }) => {
    try {
      return await createDepartmentRequest(dto)
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Departman oluşturulurken bir hata oluştu.')
    }
  }
)

const departmentSlice = createSlice({
  name: 'department',
  initialState,
  reducers: {
    resetCreateStatus: (state) => {
      state.createStatus = 'idle'
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchDepartments.pending, (state) => {
        state.status = 'loading'
      })
      .addCase(fetchDepartments.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.items = action.payload.items
        state.totalCount = action.payload.totalCount
      })
      .addCase(fetchDepartments.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
      .addCase(createDepartment.pending, (state) => {
        state.createStatus = 'loading'
        state.error = null
      })
      .addCase(createDepartment.fulfilled, (state) => {
        state.createStatus = 'succeeded'
      })
      .addCase(createDepartment.rejected, (state, action) => {
        state.createStatus = 'failed'
        state.error = action.payload as string
      })
  },
})

export const { resetCreateStatus } = departmentSlice.actions
export default departmentSlice.reducer
