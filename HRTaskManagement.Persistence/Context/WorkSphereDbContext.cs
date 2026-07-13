using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Persistence.Context
{
    public class WorkSphereDbContext : DbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public WorkSphereDbContext(
            DbContextOptions<WorkSphereDbContext> options,
            ICurrentUserService? currentUserService = null)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        // DbSet Tanımları — Her biri veritabanında bir tabloya karşılık gelir
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetAssignment> AssetAssignments { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurations klasöründeki tüm IEntityTypeConfiguration<> sınıflarını otomatik uygular
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkSphereDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = GetAuditLogs();
            if (auditEntries.Any())
            {
                Logs.AddRange(auditEntries);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private List<Log> GetAuditLogs()
        {
            var auditEntries = new List<Log>();
            var userId = _currentUserService?.UserId;
            var ipAddress = _currentUserService?.IpAddress;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                // Skip logging changes to Log entity itself to avoid infinite recursion!
                if (entry.Entity is Log)
                    continue;

                // Special handling for RefreshToken creation which represents a login action
                if (entry.Entity is RefreshToken rt && entry.State == EntityState.Added)
                {
                    var loginLog = new Log
                    {
                        UserId = rt.UserId,
                        Action = "Login",
                        EntityName = "User",
                        EntityId = rt.UserId,
                        IpAddress = ipAddress,
                        CreatedDate = DateTime.UtcNow
                    };
                    auditEntries.Add(loginLog);
                    continue;
                }

                // Skip other refresh token state changes to avoid unnecessary noise
                if (entry.Entity is RefreshToken)
                    continue;

                var action = entry.State switch
                {
                    EntityState.Added => "Create",
                    EntityState.Modified => entry.Entity.IsDeleted ? "Delete" : "Update", // Map soft-delete to Delete action
                    EntityState.Deleted => "Delete",
                    _ => null
                };

                if (action == null)
                    continue;

                var entityName = entry.Entity.GetType().Name;
                if (entityName.Contains("Proxy"))
                {
                    entityName = entry.Entity.GetType().BaseType?.Name ?? entityName;
                }

                var auditEntry = new Log
                {
                    UserId = userId,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entry.Entity.Id,
                    IpAddress = ipAddress,
                    CreatedDate = DateTime.UtcNow
                };

                auditEntries.Add(auditEntry);
            }

            return auditEntries;
        }
    }
}