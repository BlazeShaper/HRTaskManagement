// Persistence/Services/AssetAssignmentService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.AssetAssignment;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Enums;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class AssetAssignmentService : IAssetAssignmentService
    {
        private readonly WorkSphereDbContext _dbContext;

        public AssetAssignmentService(WorkSphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static IQueryable<AssetAssignmentDto> ProjectToDto(
            IQueryable<Domain.Entities.AssetAssignment> source)
        {
            return source.Select(aa => new AssetAssignmentDto
            {
                Id = aa.Id,
                AssetId = aa.AssetId,
                AssetName = aa.Asset!.Name,
                AssetSerialNumber = aa.Asset.SerialNumber,
                EmployeeId = aa.EmployeeId,
                EmployeeFullName = aa.Employee!.FirstName + " " + aa.Employee.LastName,
                AssignedByUserId = aa.AssignedByUserId,
                AssignedByUsername = aa.AssignedByUser!.Username,
                AssignedDate = aa.AssignedDate,
                ReturnedDate = aa.ReturnedDate,
                Note = aa.Note
            });
        }

        public async Task<PagedResult<AssetAssignmentDto>> GetAllAsync(
            AssetAssignmentQueryParameters queryParameters)
        {
            var query = _dbContext.AssetAssignments
                .AsNoTracking()
                .Where(aa => !aa.IsDeleted);

            if (queryParameters.EmployeeId.HasValue)
                query = query.Where(aa => aa.EmployeeId == queryParameters.EmployeeId.Value);

            if (queryParameters.AssetId.HasValue)
                query = query.Where(aa => aa.AssetId == queryParameters.AssetId.Value);

            if (queryParameters.OnlyActive == true)
                query = query.Where(aa => aa.ReturnedDate == null);

            var totalCount = await query.CountAsync();

            var items = await ProjectToDto(query)
                .OrderByDescending(aa => aa.AssignedDate)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            return new PagedResult<AssetAssignmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }

        public async Task<AssetAssignmentDto> GetByIdAsync(Guid id)
        {
            var query = _dbContext.AssetAssignments
                .AsNoTracking()
                .Where(aa => aa.Id == id && !aa.IsDeleted);

            var result = await ProjectToDto(query).FirstOrDefaultAsync();

            return result ?? throw new KeyNotFoundException("Zimmet kaydı bulunamadı.");
        }

        public async Task<AssetAssignmentDto> AssignAsync(
            CreateAssetAssignmentDto dto, Guid currentUserId)
        {
            // Demirbaş kontrolü
            var asset = await _dbContext.Assets
                .FirstOrDefaultAsync(a => a.Id == dto.AssetId && !a.IsDeleted)
                ?? throw new KeyNotFoundException("Demirbaş bulunamadı.");

            if (asset.Status != AssetStatus.Available)
                throw new InvalidOperationException(
                    "Bu demirbaş şu anda müsait değil (zimmetli veya arızalı olabilir).");

            // Çalışan kontrolü
            var employeeExists = await _dbContext.Employees
                .AnyAsync(e => e.Id == dto.EmployeeId && !e.IsDeleted && e.IsActive);

            if (!employeeExists)
                throw new KeyNotFoundException("Çalışan bulunamadı veya aktif değil.");

            // Zimmet oluştur
            var assignment = new Domain.Entities.AssetAssignment
            {
                AssetId = dto.AssetId,
                EmployeeId = dto.EmployeeId,
                AssignedByUserId = currentUserId,
                AssignedDate = DateTime.UtcNow,
                Note = dto.Note
            };

            // Demirbaş durumunu güncelle
            asset.Status = AssetStatus.Assigned;
            asset.UpdatedDate = DateTime.UtcNow;

            _dbContext.AssetAssignments.Add(assignment);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(assignment.Id);
        }

        public async Task<AssetAssignmentDto> ReturnAsync(Guid id, ReturnAssetDto dto)
        {
            var assignment = await _dbContext.AssetAssignments
                .Include(aa => aa.Asset)
                .FirstOrDefaultAsync(aa => aa.Id == id && !aa.IsDeleted)
                ?? throw new KeyNotFoundException("Zimmet kaydı bulunamadı.");

            if (assignment.ReturnedDate != null)
                throw new InvalidOperationException("Bu demirbaş zaten iade edilmiş.");

            assignment.ReturnedDate = DateTime.UtcNow;
            assignment.UpdatedDate = DateTime.UtcNow;

            // Mevcut nota iade notunu ekle
            if (!string.IsNullOrWhiteSpace(dto.ReturnNote))
            {
                assignment.Note = string.IsNullOrWhiteSpace(assignment.Note)
                    ? dto.ReturnNote
                    : $"{assignment.Note} | İade notu: {dto.ReturnNote}";
            }

            // Demirbaş durumunu serbest bırak
            assignment.Asset!.Status = AssetStatus.Available;
            assignment.Asset.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(assignment.Id);
        }
    }
}
