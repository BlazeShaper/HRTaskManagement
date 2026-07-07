// Persistence/Services/AssetService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Asset;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Domain.Enums;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class AssetService : IAssetService
    {
        private readonly WorkSphereDbContext _dbContext;

        public AssetService(WorkSphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<AssetDto>> GetAllAsync(AssetQueryParameters queryParameters)
        {
            var query = _dbContext.Assets.AsNoTracking().Where(a => !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                var term = queryParameters.SearchTerm.Trim().ToLower();
                query = query.Where(a =>
                    a.Name.ToLower().Contains(term) ||
                    a.SerialNumber.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.AssetType))
            {
                query = query.Where(a => a.AssetType.ToLower() == queryParameters.AssetType.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.Status) &&
                Enum.TryParse<AssetStatus>(queryParameters.Status, true, out var statusFilter))
            {
                query = query.Where(a => a.Status == statusFilter);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.Name)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(a => new AssetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    AssetType = a.AssetType,
                    SerialNumber = a.SerialNumber,
                    PurchaseDate = a.PurchaseDate,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            return new PagedResult<AssetDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }

        public async Task<AssetDto> GetByIdAsync(Guid id)
        {
            var asset = await _dbContext.Assets
                .AsNoTracking()
                .Where(a => a.Id == id && !a.IsDeleted)
                .Select(a => new AssetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    AssetType = a.AssetType,
                    SerialNumber = a.SerialNumber,
                    PurchaseDate = a.PurchaseDate,
                    Status = a.Status.ToString()
                })
                .FirstOrDefaultAsync();

            return asset ?? throw new KeyNotFoundException("Demirbaş bulunamadı.");
        }

        public async Task<AssetDto> CreateAsync(CreateAssetDto dto)
        {
            var serialExists = await _dbContext.Assets
                .AnyAsync(a => a.SerialNumber.ToLower() == dto.SerialNumber.Trim().ToLower() && !a.IsDeleted);

            if (serialExists)
                throw new InvalidOperationException("Bu seri numarasına sahip bir demirbaş zaten mevcut.");

            var asset = new Asset
            {
                Name = dto.Name.Trim(),
                AssetType = dto.AssetType.Trim(),
                SerialNumber = dto.SerialNumber.Trim(),
                PurchaseDate = dto.PurchaseDate,
                Status = AssetStatus.Available // Yeni demirbaş her zaman "Available" olarak başlar
            };

            _dbContext.Assets.Add(asset);
            await _dbContext.SaveChangesAsync();

            return new AssetDto
            {
                Id = asset.Id,
                Name = asset.Name,
                AssetType = asset.AssetType,
                SerialNumber = asset.SerialNumber,
                PurchaseDate = asset.PurchaseDate,
                Status = asset.Status.ToString()
            };
        }

        public async Task<AssetDto> UpdateAsync(Guid id, UpdateAssetDto dto)
        {
            var asset = await _dbContext.Assets
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted)
                ?? throw new KeyNotFoundException("Demirbaş bulunamadı.");

            if (!Enum.TryParse<AssetStatus>(dto.Status, true, out var newStatus))
                throw new InvalidOperationException("Geçersiz durum değeri.");

            // Elle "Assigned" yapılamaz — bu durum yalnızca AssetAssignment akışı üzerinden değişmeli
            if (newStatus == AssetStatus.Assigned && asset.Status != AssetStatus.Assigned)
                throw new InvalidOperationException(
                    "Bir demirbaşı doğrudan 'Assigned' yapamazsınız, bunun için AssetAssignment akışını kullanın.");

            asset.Name = dto.Name.Trim();
            asset.AssetType = dto.AssetType.Trim();
            asset.PurchaseDate = dto.PurchaseDate;
            asset.Status = newStatus;
            asset.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new AssetDto
            {
                Id = asset.Id,
                Name = asset.Name,
                AssetType = asset.AssetType,
                SerialNumber = asset.SerialNumber,
                PurchaseDate = asset.PurchaseDate,
                Status = asset.Status.ToString()
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var asset = await _dbContext.Assets
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted)
                ?? throw new KeyNotFoundException("Demirbaş bulunamadı.");

            if (asset.Status == AssetStatus.Assigned)
                throw new InvalidOperationException("Şu anda bir çalışana zimmetli demirbaş silinemez.");

            asset.IsDeleted = true;
            asset.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }
}
