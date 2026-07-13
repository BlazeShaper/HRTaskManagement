// Application/Interfaces/ITaskService.cs
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Task;

namespace HRTaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<PagedResult<TaskDto>> GetAllAsync(TaskQueryParameters queryParameters);
        System.Threading.Tasks.Task<TaskDto> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<TaskDto> CreateAsync(CreateTaskDto request);
        System.Threading.Tasks.Task UpdateAsync(Guid id, UpdateTaskDto request);
        System.Threading.Tasks.Task UpdateStatusAsync(Guid id, UpdateTaskStatusDto request);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
    }
}