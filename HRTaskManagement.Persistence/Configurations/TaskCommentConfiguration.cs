using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRTaskManagement.Domain.Entities;

namespace HRTaskManagement.Persistence.Configurations
{
    public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.ToTable("TaskComments");

            builder.Property(tc => tc.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(tc => tc.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tc => tc.Employee)
                .WithMany(e => e.TaskComments)
                .HasForeignKey(tc => tc.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
