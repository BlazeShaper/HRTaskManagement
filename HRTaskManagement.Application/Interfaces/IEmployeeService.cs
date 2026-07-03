// Application/Interfaces/IEmployeeService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Employee;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(Guid id);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task<bool> UpdateAsync(Guid id, UpdateEmployeeDto updateEmployeeDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
