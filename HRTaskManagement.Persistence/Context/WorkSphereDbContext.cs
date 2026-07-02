using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Domain.Entities;

namespace HRTaskManagement.Persistence.Context
{
    public class WorkSphereDbContext : DbContext
    {
        public WorkSphereDbContext(DbContextOptions<WorkSphereDbContext> options)
            : base(options)
        {
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurations klasöründeki tüm IEntityTypeConfiguration<> sınıflarını otomatik uygular
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkSphereDbContext).Assembly);
        }
    }
}