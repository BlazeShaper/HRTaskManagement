// src/types/notification.ts
export type NotificationType = 'TaskAssigned' | 'LeaveRequestApproved' | 'LeaveRequestRejected' | 'General'

export interface NotificationDto {
  id: string
  title: string
  message: string
  type: NotificationType
  isRead: boolean
  createdDate: string
}
