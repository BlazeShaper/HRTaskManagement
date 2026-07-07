using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Asset;
using HRTaskManagement.Application.DTOs.Common;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IAssetService
    {
        Task<PagedResult<AssetDto>> GetAllAsync(AssetQueryParameters queryParameters);
        Task<AssetDto> GetByIdAsync(Guid id);
        Task<AssetDto> CreateAsync(CreateAssetDto dto);
        Task<AssetDto> UpdateAsync(Guid id, UpdateAssetDto dto);
        Task DeleteAsync(Guid id);
    }
}
