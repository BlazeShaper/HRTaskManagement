// src/store/slices/positionSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchPositionsRequest, createPositionRequest } from '../../api/positionApi'
import type { PositionDto, CreatePositionDto, PositionQueryParameters } from '../../types/position'

interface PositionState {
  items: PositionDto[]
  totalCount: number
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  createStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: PositionState = {
  items: [],
  totalCount: 0,
  status: 'idle',
  createStatus: 'idle',
  error: null,
}

export const fetchPositions = createAsyncThunk(
  'position/fetchPositions',
  async (params: PositionQueryParameters, { rejectWithValue }) => {
    try {
      return await fetchPositionsRequest(params)
    } catch {
      return rejectWithValue('Pozisyonlar yüklenirken bir hata oluştu.')
    }
  }
)

export const createPosition = createAsyncThunk(
  'position/createPosition',
  async (dto: CreatePositionDto, { rejectWithValue }) => {
    try {
      return await createPositionRequest(dto)
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Pozisyon oluşturulurken bir hata oluştu.')
    }
  }
)

const positionSlice = createSlice({
  name: 'position',
  initialState,
  reducers: {
    resetCreateStatus: (state) => {
      state.createStatus = 'idle'
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPositions.pending, (state) => {
        state.status = 'loading'
      })
      .addCase(fetchPositions.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.items = action.payload.items
        state.totalCount = action.payload.totalCount
      })
      .addCase(fetchPositions.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
      .addCase(createPosition.pending, (state) => {
        state.createStatus = 'loading'
        state.error = null
      })
      .addCase(createPosition.fulfilled, (state) => {
        state.createStatus = 'succeeded'
      })
      .addCase(createPosition.rejected, (state, action) => {
        state.createStatus = 'failed'
        state.error = action.payload as string
      })
  },
})

export const { resetCreateStatus } = positionSlice.actions
export default positionSlice.reducer
