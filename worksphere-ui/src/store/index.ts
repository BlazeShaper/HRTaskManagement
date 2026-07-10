// src/store/index.ts
import { configureStore } from '@reduxjs/toolkit'
import uiReducer from './slices/uiSlice'
import authReducer from './slices/authSlice'
import employeeReducer from './slices/employeeSlice'
import taskReducer from './slices/taskSlice'

export const store = configureStore({
    reducer: {
        ui: uiReducer,
        auth: authReducer,
        employee: employeeReducer,
        task: taskReducer,
    },
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch