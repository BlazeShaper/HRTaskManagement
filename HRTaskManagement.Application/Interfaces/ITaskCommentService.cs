using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.TaskComment;

namespace HRTaskManagement.Application.Interfaces
{
    public interface ITaskCommentService
    {
        Task<PagedResult<TaskCommentDto>> GetAllAsync(TaskCommentQueryParameters queryParameters);
        Task<TaskCommentDto> GetByIdAsync(Guid id);
        Task<TaskCommentDto> CreateAsync(CreateTaskCommentDto dto, Guid currentUserId);
        Task DeleteAsync(Guid id, Guid currentUserId, bool isManagerOrAbove);
    }
}
