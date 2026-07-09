// src/store/slices/authSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { jwtDecode } from 'jwt-decode'
import { loginRequest } from '../../api/authApi'
import type { User, LoginRequest, DecodedToken } from '../../types/auth'

interface AuthState {
    user: User | null
    accessToken: string | null
    refreshToken: string | null
    status: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

const initialState: AuthState = {
    user: null,
    accessToken: null,
    refreshToken: null,
    status: 'idle',
    error: null,
}

export const loginUser = createAsyncThunk(
    'auth/loginUser',
    async (credentials: LoginRequest, { rejectWithValue }) => {
        try {
            return await loginRequest(credentials)
        } catch {
            return rejectWithValue('Kullanıcı adı veya şifre hatalı.')
        }
    }
)

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        logout: (state) => {
            state.user = null
            state.accessToken = null
            state.refreshToken = null
            localStorage.removeItem('accessToken')
            localStorage.removeItem('refreshToken')
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(loginUser.pending, (state) => {
                state.status = 'loading'
                state.error = null
            })
            .addCase(loginUser.fulfilled, (state, action) => {
                const { accessToken, refreshToken } = action.payload
                const decoded = jwtDecode<DecodedToken>(accessToken)

                const user: User = {
                    id: decoded.sub,
                    username: decoded.unique_name,
                    roles: Array.isArray(decoded.role) ? decoded.role : [decoded.role],
                }

                state.status = 'succeeded'
                state.user = user
                state.accessToken = accessToken
                state.refreshToken = refreshToken

                localStorage.setItem('accessToken', accessToken)
                localStorage.setItem('refreshToken', refreshToken)
            })
            .addCase(loginUser.rejected, (state, action) => {
                state.status = 'failed'
                state.error = action.payload as string
            })
    },
})

export const { logout } = authSlice.actions
export default authSlice.reducer
