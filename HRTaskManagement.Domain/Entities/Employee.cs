using System;
using System.Collections.Generic;
using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
	public class Employee : BaseEntity   // Id buradan miras geliyor, tekrar yazmıyoruz
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public DateOnly BirthDate { get; set; }
		public DateTime HireDate { get; set; }
		public Guid DepartmentId { get; set; }
		public Guid PositionId { get; set; }
		public bool IsActive { get; set; } = true;
		public string? ImageUrl { get; set; }
        public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();

        // Navigation Properties (ilişkili entity'lere referans)
        public User? User { get; set; }
		public Department? Department { get; set; }
		public Position? Position { get; set; }
		public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
	}
}