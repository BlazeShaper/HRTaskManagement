// src/store/slices/notificationSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { fetchUnreadNotificationsRequest, markNotificationAsReadRequest } from '../../api/notificationApi'
import type { NotificationDto } from '../../types/notification'

interface NotificationState {
  items: NotificationDto[]
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: NotificationState = {
  items: [],
  status: 'idle',
  error: null,
}

export const fetchUnreadNotifications = createAsyncThunk(
  'notification/fetchUnread',
  async (_, { rejectWithValue }) => {
    try {
      const result = await fetchUnreadNotificationsRequest()
      return result.items
    } catch {
      return rejectWithValue('Bildirimler yüklenirken bir hata oluştu.')
    }
  }
)

export const markNotificationAsRead = createAsyncThunk(
  'notification/markAsRead',
  async (id: string, { rejectWithValue }) => {
    try {
      await markNotificationAsReadRequest(id)
      return id
    } catch {
      return rejectWithValue('Bildirim güncellenirken bir hata oluştu.')
    }
  }
)

const notificationSlice = createSlice({
  name: 'notification',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchUnreadNotifications.pending, (state) => {
        state.status = 'loading'
      })
      .addCase(fetchUnreadNotifications.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.items = action.payload
      })
      .addCase(fetchUnreadNotifications.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
      .addCase(markNotificationAsRead.fulfilled, (state, action) => {
        state.items = state.items.filter((n) => n.id !== action.payload)
      })
  },
})

export default notificationSlice.reducer
