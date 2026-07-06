// Application/Interfaces/ITaskService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Task;

namespace HRTaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetAllAsync();
        System.Threading.Tasks.Task<TaskDto> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<TaskDto> CreateAsync(CreateTaskDto request);
        System.Threading.Tasks.Task UpdateAsync(Guid id, UpdateTaskDto request);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
    }
}