// src/api/axiosInstance.ts
import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios'

const axiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
})

axiosInstance.interceptors.request.use(
    (config) => {
        const accessToken = localStorage.getItem('accessToken')

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`
        }

        return config
    },
    (error) => {
        return Promise.reject(error)
    }
)

// --- 401 Retry with Refresh Token ---

let _isRefreshing = false
let _failedQueue: {
    resolve: (token: string) => void
    reject: (error: unknown) => void
}[] = []

function processQueue(error: unknown, token: string | null) {
    _failedQueue.forEach(({ resolve, reject }) => {
        if (token) {
            resolve(token)
        } else {
            reject(error)
        }
    })
    _failedQueue = []
}

axiosInstance.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean }

        // If the failed request is the refresh call itself, don't retry
        if (
            error.response?.status === 401 &&
            originalRequest &&
            !originalRequest._retry &&
            !originalRequest.url?.includes('/auth/refresh')
        ) {
            if (_isRefreshing) {
                // Another refresh is in progress — queue this request
                return new Promise<string>((resolve, reject) => {
                    _failedQueue.push({ resolve, reject })
                }).then((newToken) => {
                    originalRequest.headers.Authorization = `Bearer ${newToken}`
                    return axiosInstance(originalRequest)
                })
            }

            originalRequest._retry = true
            _isRefreshing = true

            try {
                const response = await axios.post<{ accessToken: string }>(
                    `${import.meta.env.VITE_API_BASE_URL}/auth/refresh`,
                    {},
                    { withCredentials: true }
                )
                const data = response.data

                localStorage.setItem('accessToken', data.accessToken)
                originalRequest.headers.Authorization = `Bearer ${data.accessToken}`

                processQueue(null, data.accessToken)

                return axiosInstance(originalRequest)
            } catch (refreshError) {
                processQueue(refreshError, null)

                localStorage.removeItem('accessToken')
                window.location.href = '/login'

                return Promise.reject(refreshError)
            } finally {
                _isRefreshing = false
            }
        }

        return Promise.reject(error)
    }
)

export default axiosInstance