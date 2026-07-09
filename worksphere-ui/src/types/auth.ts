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
    refreshToken: string
}

export interface DecodedToken {
    sub: string
    unique_name: string
    role: string | string[]
    exp: number
}