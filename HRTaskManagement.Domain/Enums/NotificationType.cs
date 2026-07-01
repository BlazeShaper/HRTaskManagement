// Enums/NotificationType.cs
namespace HRTaskManagement.Domain.Enums
{
    public enum NotificationType
    {
        TaskAssigned,       // Görev atandı
        TaskCompleted,      // Görev tamamlandı
        LeaveApproved,      // İzin onaylandı
        LeaveRejected,      // İzin reddedildi
        CommentAdded,       // Yorum eklendi
        AssetAssigned,      // Varlık atandı
        General              // Genel bildirim
    }
}