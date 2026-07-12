// src/store/slices/authSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { jwtDecode } from 'jwt-decode'
import { loginRequest, logoutRequest } from '../../api/authApi'
import type { User, LoginRequest, LoginResponse, DecodedToken } from '../../types/auth'

interface AuthState {
    user: User | null
    accessToken: string | null
    status: 'idle' | 'loading' | 'succeeded' | 'failed'
    error: string | null
}

function loadAuthFromStorage(): AuthState {
    const accessToken = localStorage.getItem('accessToken')

    if (!accessToken) {
        console.log('[AuthSlice] No accessToken found in localStorage — starting as unauthenticated.')
        return {
            user: null,
            accessToken: null,
            status: 'idle',
            error: null,
        }
    }

    try {
        const decoded = jwtDecode<DecodedToken>(accessToken)

        // Check if token is expired
        const now = Date.now() / 1000
        if (decoded.exp < now) {
            console.log('[AuthSlice] accessToken expired — clearing localStorage.')
            localStorage.removeItem('accessToken')
            return {
                user: null,
                accessToken: null,
                status: 'idle',
                error: null,
            }
        }

        const roleClaim = decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        const roles = Array.isArray(roleClaim) ? roleClaim : roleClaim ? [roleClaim] : []
        const user: User = {
            id: decoded.sub,
            username: decoded.unique_name,
            roles: roles as string[],
        }

        console.log('[AuthSlice] Restored session from localStorage:', { username: user.username, roles: user.roles })

        return {
            user,
            accessToken,
            status: 'succeeded',
            error: null,
        }
    } catch (err) {
        console.error('[AuthSlice] Failed to decode accessToken — clearing localStorage.', err)
        localStorage.removeItem('accessToken')
        return {
            user: null,
            accessToken: null,
            status: 'idle',
            error: null,
        }
    }
}

const initialState: AuthState = loadAuthFromStorage()

export const loginUser = createAsyncThunk<
    LoginResponse,
    LoginRequest,
    { rejectValue: string }
>(
    'auth/loginUser',
    async (credentials, { rejectWithValue }) => {
        try {
            return await loginRequest(credentials)
        } catch {
            return rejectWithValue('Kullanıcı adı veya şifre hatalı.')
        }
    }
)

export const logoutUser = createAsyncThunk(
    'auth/logoutUser',
    async () => {
        try {
            await logoutRequest()
        } catch {
            // Backend'e ulaşılamasa bile local state'i temizle
        }
    }
)

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(loginUser.pending, (state) => {
                state.status = 'loading'
                state.error = null
            })
            .addCase(loginUser.fulfilled, (state, action) => {
                const { accessToken } = action.payload
                const decoded = jwtDecode<DecodedToken>(accessToken)

                const roleClaim = decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
                const roles = Array.isArray(roleClaim) ? roleClaim : roleClaim ? [roleClaim] : []
                const user: User = {
                    id: decoded.sub,
                    username: decoded.unique_name,
                    roles: roles as string[],
                }

                state.status = 'succeeded'
                state.user = user
                state.accessToken = accessToken

                localStorage.setItem('accessToken', accessToken)
            })
            .addCase(loginUser.rejected, (state, action) => {
                state.status = 'failed'
                state.error = action.payload ?? 'Bir hata oluştu.'
            })
            .addCase(logoutUser.fulfilled, (state) => {
                state.user = null
                state.accessToken = null
                state.status = 'idle'
                state.error = null
                localStorage.removeItem('accessToken')
            })
    },
})

export default authSlice.reducer
