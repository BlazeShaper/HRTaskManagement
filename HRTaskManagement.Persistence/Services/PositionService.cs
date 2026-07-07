// Persistence/Services/PositionService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Position;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class PositionService : IPositionService
    {
        private readonly WorkSphereDbContext _context;

        public PositionService(WorkSphereDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PositionDto>> GetAllAsync(PositionQueryParameters queryParameters)
        {
            IQueryable<Position> query = _context.Positions
                .Include(p => p.Employees.Where(e => !e.IsDeleted))
                .AsNoTracking();

            // Arama — başlık içinde ara
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                var search = queryParameters.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var positions = await query
                .OrderBy(p => p.Title)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(p => new PositionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    EmployeeCount = p.Employees.Count(e => !e.IsDeleted)
                })
                .ToListAsync();

            return new PagedResult<PositionDto>
            {
                Items = positions,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PositionDto> GetByIdAsync(Guid id)
        {
            var position = await _context.Positions
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PositionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    EmployeeCount = p.Employees.Count(e => !e.IsDeleted)
                })
                .FirstOrDefaultAsync();

            if (position == null)
                throw new KeyNotFoundException($"{id} ID'li pozisyon bulunamadı.");

            return position;
        }

        public async Task<PositionDto> CreateAsync(CreatePositionDto dto)
        {
            bool titleExists = await _context.Positions
                .AnyAsync(p => p.Title == dto.Title);

            if (titleExists)
                throw new InvalidOperationException($"'{dto.Title}' başlıklı bir pozisyon zaten mevcut.");

            var position = new Position
            {
                Title = dto.Title,
                Description = dto.Description
            };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(position.Id);
        }

        public async Task<PositionDto> UpdateAsync(Guid id, UpdatePositionDto dto)
        {
            var position = await _context.Positions.FindAsync(id);

            if (position == null)
                throw new KeyNotFoundException($"{id} ID'li pozisyon bulunamadı.");

            if (position.Title != dto.Title)
            {
                bool titleExists = await _context.Positions
                    .AnyAsync(p => p.Title == dto.Title && p.Id != id);

                if (titleExists)
                    throw new InvalidOperationException($"'{dto.Title}' başlıklı bir pozisyon zaten mevcut.");
            }

            position.Title = dto.Title;
            position.Description = dto.Description;
            position.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var position = await _context.Positions
                .Include(p => p.Employees.Where(e => !e.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (position == null)
                throw new KeyNotFoundException($"{id} ID'li pozisyon bulunamadı.");

            if (position.Employees.Any())
                throw new InvalidOperationException(
                    "Bu pozisyonda hâlâ aktif çalışanlar bulunuyor. Silmeden önce çalışanları başka bir pozisyona taşıyın.");

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
        }
    }
}
