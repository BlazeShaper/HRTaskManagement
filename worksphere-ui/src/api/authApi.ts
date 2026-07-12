// src/api/authApi.ts
import axiosInstance from './axiosInstance'
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from '../types/auth'

export const loginRequest = async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosInstance.post<LoginResponse>('/auth/login', credentials)
    return response.data
}

export const registerRequest = async (data: RegisterRequest): Promise<RegisterResponse> => {
    const response = await axiosInstance.post<RegisterResponse>('/auth/register', data)
    return response.data
}
export const logoutRequest = async (): Promise<void> => {
    await axiosInstance.post('/auth/logout')
}
