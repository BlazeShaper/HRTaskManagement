using HRTaskManagement.Domain.Common;
// Entities/Department.cs
namespace HRTaskManagement.Domain.Entities
{
	public class Department : BaseEntity
	{
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }

		// Navigation Property — Bir departmanda birden çok çalışan olur
		public ICollection<Employee> Employees { get; set; } = new List<Employee>();
	}
}
