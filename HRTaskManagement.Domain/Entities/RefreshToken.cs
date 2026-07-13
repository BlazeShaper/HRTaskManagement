using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        // Reuse detection zinciri için: bu token rotate edildiğinde
        // yerine üretilen yeni token'ın değeri burada tutulur.
        public string? ReplacedByToken { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}