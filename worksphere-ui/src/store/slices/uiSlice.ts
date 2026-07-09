// src/store/slices/uiSlice.ts
import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

interface UiState {
    isSidebarCollapsed: boolean
}

const initialState: UiState = {
    isSidebarCollapsed: false,
}

const uiSlice = createSlice({
    name: 'ui',
    initialState,
    reducers: {
        toggleSidebar: (state) => {
            state.isSidebarCollapsed = !state.isSidebarCollapsed
        },
        setSidebarCollapsed: (state, action: PayloadAction<boolean>) => {
            state.isSidebarCollapsed = action.payload
        },
    },
})

export const { toggleSidebar, setSidebarCollapsed } = uiSlice.actions
export default uiSlice.reducer