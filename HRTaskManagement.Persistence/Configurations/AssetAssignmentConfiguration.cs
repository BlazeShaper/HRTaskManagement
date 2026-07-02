using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRTaskManagement.Domain.Entities;

namespace HRTaskManagement.Persistence.Configurations
{
    public class AssetAssignmentConfiguration : IEntityTypeConfiguration<AssetAssignment>
    {
        public void Configure(EntityTypeBuilder<AssetAssignment> builder)
        {
            builder.ToTable("AssetAssignments");

            builder.Property(aa => aa.Note)
                .HasMaxLength(500);

            builder.HasOne(aa => aa.Asset)
                .WithMany(a => a.AssetAssignments)
                .HasForeignKey(aa => aa.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(aa => aa.Employee)
                .WithMany(e => e.AssetAssignments)
                .HasForeignKey(aa => aa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(aa => aa.AssignedByUser)
                .WithMany()
                .HasForeignKey(aa => aa.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
