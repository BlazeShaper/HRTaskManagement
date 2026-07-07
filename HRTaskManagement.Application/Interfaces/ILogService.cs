using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Log;

namespace HRTaskManagement.Application.Interfaces
{
    public interface ILogService
    {
        Task<PagedResult<LogDto>> GetAllAsync(LogQueryParameters queryParameters);
        Task<LogDto> GetByIdAsync(Guid id);
        Task LogActionAsync(Guid? userId, string action, string entityName, Guid entityId, string? ipAddress);
    }
}
