// src/api/notificationApi.ts
import axiosInstance from './axiosInstance'
import type { NotificationDto } from '../types/notification'
import type { PagedResult } from '../types/common'

export const fetchUnreadNotificationsRequest = async (): Promise<PagedResult<NotificationDto>> => {
  const response = await axiosInstance.get<PagedResult<NotificationDto>>('/notification', {
    params: { isRead: false },
  })
  return response.data
}

export const markNotificationAsReadRequest = async (id: string): Promise<void> => {
  await axiosInstance.patch(`/notification/${id}/read`)
}
