// Application/Interfaces/IDepartmentService.cs
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Department;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<PagedResult<DepartmentDto>> GetAllAsync(DepartmentQueryParameters queryParameters);
        Task<DepartmentDto> GetByIdAsync(Guid id);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto request);
        Task UpdateAsync(Guid id, UpdateDepartmentDto request);
        Task DeleteAsync(Guid id);
    }
}