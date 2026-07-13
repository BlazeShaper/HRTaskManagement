using HRTaskManagement.Application.DTOs.AssetAssignment;
using HRTaskManagement.Application.DTOs.Common;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IAssetAssignmentService
    {
        Task<PagedResult<AssetAssignmentDto>> GetAllAsync(AssetAssignmentQueryParameters queryParameters);
        Task<AssetAssignmentDto> GetByIdAsync(Guid id);
        Task<AssetAssignmentDto> AssignAsync(CreateAssetAssignmentDto dto, Guid currentUserId);
        Task<AssetAssignmentDto> ReturnAsync(Guid id, ReturnAssetDto dto);
    }
}
