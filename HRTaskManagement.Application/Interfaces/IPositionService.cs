using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Position;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IPositionService
    {
        Task<PagedResult<PositionDto>> GetAllAsync(PositionQueryParameters queryParameters);
        Task<PositionDto> GetByIdAsync(Guid id);
        Task<PositionDto> CreateAsync(CreatePositionDto dto);
        Task<PositionDto> UpdateAsync(Guid id, UpdatePositionDto dto);
        Task DeleteAsync(Guid id);
    }
}