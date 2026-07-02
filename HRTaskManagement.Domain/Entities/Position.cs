using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
	public class Position : BaseEntity
	{
		public string Title { get; set; } = string.Empty;   // Örn: "Yazılım Geliştirici"
		public string? Description { get; set; }

		public ICollection<Employee> Employees { get; set; } = new List<Employee>();
	}
}