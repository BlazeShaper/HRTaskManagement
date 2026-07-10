// src/api/taskApi.ts
import axiosInstance from './axiosInstance'
import type { TaskDto, TaskQueryParameters } from '../types/task'
import type { PagedResult } from '../types/common'

export const fetchTasksRequest = async (
    params: TaskQueryParameters
): Promise<PagedResult<TaskDto>> => {
    const response = await axiosInstance.get<PagedResult<TaskDto>>('/task', { params })
    return response.data
}