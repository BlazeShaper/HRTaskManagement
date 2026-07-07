using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.LeaveRequest;

namespace HRTaskManagement.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<PagedResult<LeaveRequestDto>> GetAllAsync(LeaveRequestQueryParameters queryParameters);
        Task<LeaveRequestDto> GetByIdAsync(Guid id);
        Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, Guid currentUserId);
        Task<LeaveRequestDto> ApproveAsync(Guid id, Guid approverUserId);
        Task<LeaveRequestDto> RejectAsync(Guid id, Guid approverUserId);
        Task<LeaveRequestDto> CancelAsync(Guid id, Guid currentUserId);
    }
}
