// Persistence/Services/DepartmentService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Department;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
	public class DepartmentService : IDepartmentService
	{
		private readonly WorkSphereDbContext _context;

		public DepartmentService(WorkSphereDbContext context)
		{
			_context = context;
		}

		public async Task<DepartmentDto> GetByIdAsync(Guid id)
		{
			var department = await _context.Departments
				.AsNoTracking()
				.Where(d => d.Id == id)
				.Select(d => new DepartmentDto
				{
					Id = d.Id,
					Name = d.Name,
					Description = d.Description,
					EmployeeCount = d.Employees.Count(),
					ManagerId = d.ManagerId,
					ManagerFullName = d.Manager != null
						? d.Manager.FirstName + " " + d.Manager.LastName
						: null
				})
				.FirstOrDefaultAsync();

			if (department == null)
				throw new KeyNotFoundException("Departman bulunamadı.");

			return department;
		}

		public async Task<PagedResult<DepartmentDto>> GetAllAsync(DepartmentQueryParameters queryParameters)
		{
			IQueryable<Department> query = _context.Departments
				.AsNoTracking();

			// Arama — Adı veya açıklaması içinde ara
			if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
			{
				var search = queryParameters.SearchTerm.Trim().ToLower();
				query = query.Where(d => d.Name.ToLower().Contains(search) || 
				                         (d.Description != null && d.Description.ToLower().Contains(search)));
			}

			// Filtre — Yönetici Id'sine göre filtreleme
			if (queryParameters.ManagerId.HasValue)
			{
				query = query.Where(d => d.ManagerId == queryParameters.ManagerId.Value);
			}

			int totalCount = await query.CountAsync();

			var departments = await query
				.OrderBy(d => d.Name)
				.Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
				.Take(queryParameters.PageSize)
				.Select(d => new DepartmentDto
				{
					Id = d.Id,
					Name = d.Name,
					Description = d.Description,
					EmployeeCount = d.Employees.Count(),
					ManagerId = d.ManagerId,
					ManagerFullName = d.Manager != null
						? d.Manager.FirstName + " " + d.Manager.LastName
						: null
				})
				.ToListAsync();

			return new PagedResult<DepartmentDto>
			{
				Items = departments,
				PageNumber = queryParameters.PageNumber,
				PageSize = queryParameters.PageSize,
				TotalCount = totalCount
			};
		}

		public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto request)
		{
			bool nameExists = await _context.Departments
				.AnyAsync(d => d.Name == request.Name);

			if (nameExists)
				throw new InvalidOperationException($"'{request.Name}' adında bir departman zaten mevcut.");

			// ============================================
			// ANA İŞ KURALI: ManagerId geçerli bir Employee mi?
			// ============================================
			await ValidateManagerIdAsync(request.ManagerId);

			var department = new Department
			{
				Name = request.Name,
				Description = request.Description,
				ManagerId = request.ManagerId
			};

			_context.Departments.Add(department);
			await _context.SaveChangesAsync();

			return await GetByIdAsync(department.Id);
		}

		public async Task UpdateAsync(Guid id, UpdateDepartmentDto request)
		{
			var department = await _context.Departments.FindAsync(id);

			if (department == null)
				throw new KeyNotFoundException("Departman bulunamadı.");

			if (department.Name != request.Name)
			{
				bool nameExists = await _context.Departments
					.AnyAsync(d => d.Name == request.Name && d.Id != id);

				if (nameExists)
					throw new InvalidOperationException($"'{request.Name}' adında bir departman zaten mevcut.");
			}

			// ============================================
			// ANA İŞ KURALI: ManagerId geçerli bir Employee mi?
			// ============================================
			await ValidateManagerIdAsync(request.ManagerId);

			department.Name = request.Name;
			department.Description = request.Description;
			department.ManagerId = request.ManagerId;

			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var department = await _context.Departments
				.Include(d => d.Employees)
				.FirstOrDefaultAsync(d => d.Id == id);

			if (department == null)
				throw new KeyNotFoundException("Departman bulunamadı.");

			if (department.Employees.Any())
				throw new InvalidOperationException(
					"Bu departmanda hâlâ çalışanlar bulunuyor. Silmeden önce çalışanları başka bir departmana taşıyın.");

			_context.Departments.Remove(department);
			await _context.SaveChangesAsync();
		}

		// ============================================
		// Yardımcı Metod: ManagerId Doğrulama
		// ============================================
		private async Task ValidateManagerIdAsync(Guid? managerId)
		{
			// null gönderildiyse, "yönetici atanmadı" demektir, geçerlidir
			if (managerId == null)
				return;

			bool managerExists = await _context.Employees
				.AnyAsync(e => e.Id == managerId.Value && e.IsActive);

			if (!managerExists)
				throw new InvalidOperationException(
					"Belirtilen ManagerId'ye sahip aktif bir çalışan bulunamadı.");
		}

		private static DepartmentDto MapToDto(Department department)
		{
			return new DepartmentDto
			{
				Id = department.Id,
				Name = department.Name,
				Description = department.Description,
				EmployeeCount = department.Employees.Count,
				ManagerId = department.ManagerId,
				ManagerFullName = department.Manager != null
					? $"{department.Manager.FirstName} {department.Manager.LastName}"
					: null
			};
		}
	}
}