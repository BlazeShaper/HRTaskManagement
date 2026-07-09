// src/api/authApi.ts
import axiosInstance from './axiosInstance'
import type { LoginRequest, LoginResponse } from '../types/auth'

export const loginRequest = async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosInstance.post<LoginResponse>('/auth/login', credentials)
    return response.data
}
