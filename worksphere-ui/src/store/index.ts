// src/store/index.ts
import { configureStore } from '@reduxjs/toolkit'
import uiReducer from './slices/uiSlice'
import authReducer from './slices/authSlice'
import employeeReducer from './slices/employeeSlice'
import taskReducer from './slices/taskSlice'
import leaveRequestReducer from './slices/leaveRequestSlice'
import assetReducer from './slices/assetSlice'
import assetAssignmentReducer from './slices/assetAssignmentSlice'
import notificationReducer from './slices/notificationSlice'
import departmentReducer from './slices/departmentSlice'
import positionReducer from './slices/positionSlice'

export const store = configureStore({
    reducer: {
        ui: uiReducer,
        auth: authReducer,
        employee: employeeReducer,
        task: taskReducer,
        leaveRequest: leaveRequestReducer,
        asset: assetReducer,
        assetAssignment: assetAssignmentReducer,
        notification: notificationReducer,
        department: departmentReducer,
        position: positionReducer,
    },
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch