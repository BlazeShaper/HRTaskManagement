using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRTaskManagement.Domain.Entities;

namespace HRTaskManagement.Persistence.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs");

            builder.Property(l => l.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.IpAddress)
                .HasMaxLength(45); // IPv6 için yeterli uzunluk

            builder.HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
