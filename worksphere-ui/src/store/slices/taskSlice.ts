// src/store/slices/taskSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchTasksRequest } from '../../api/taskApi'
import type { TaskDto, TaskQueryParameters } from '../../types/task'

interface TaskState {
    items: TaskDto[]
    totalCount: number
    pageNumber: number
    pageSize: number
    status: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

const initialState: TaskState = {
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    status: 'idle',
    error: null,
}

export const fetchTasks = createAsyncThunk(
    'task/fetchTasks',
    async (params: TaskQueryParameters, { rejectWithValue }) => {
        try {
            return await fetchTasksRequest(params)
        } catch {
            return rejectWithValue('Görevler yüklenirken bir hata oluştu.')
        }
    }
)

const taskSlice = createSlice({
    name: 'task',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchTasks.pending, (state) => {
                state.status = 'loading'
                state.error = null
            })
            .addCase(fetchTasks.fulfilled, (state, action) => {
                state.status = 'succeeded'
                state.items = action.payload.items
                state.totalCount = action.payload.totalCount
                state.pageNumber = action.payload.pageNumber
                state.pageSize = action.payload.pageSize
            })
            .addCase(fetchTasks.rejected, (state, action) => {
                state.status = 'failed'
                state.error = action.payload as string
            })
    },
})

export default taskSlice.reducer