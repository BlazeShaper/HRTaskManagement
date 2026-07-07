// Application/Interfaces/IEmployeeService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Employee;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResult<EmployeeDto>> GetAllAsync(EmployeeQueryParameters parameters);
        Task<EmployeeDto> GetByIdAsync(Guid id);
        Task<CreateEmployeeResultDto> CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(Guid id, UpdateEmployeeDto updateEmployeeDto);
        Task DeleteAsync(Guid id);
    }
}
