// src/types/auth.ts
export interface User {
    id: string
    username: string
    roles: string[]
}

export interface LoginRequest {
    username: string
    password: string
}

export interface LoginResponse {
    accessToken: string
}

export interface DecodedToken {
    sub: string
    unique_name: string
    role?: string | string[]
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[]
    exp: number
}

export interface RegisterRequest {
    username: string
    email: string
    password: string
    role: string
}

export interface RegisterResponse {
    userId: string
    username: string
    email: string
    assignedRole: string
}