// Application/Interfaces/IDepartmentService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Department;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto> GetByIdAsync(Guid id);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto request);
        Task UpdateAsync(Guid id, UpdateDepartmentDto request);
        Task DeleteAsync(Guid id);
    }
}