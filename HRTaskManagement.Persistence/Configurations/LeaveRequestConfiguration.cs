using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRTaskManagement.Domain.Entities;

namespace HRTaskManagement.Persistence.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");

            builder.Property(lr => lr.Reason)
                .HasMaxLength(500);

            builder.Property(lr => lr.LeaveType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(lr => lr.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.ApprovedByUser)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
