// Persistence/Services/EmployeeService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using HRTaskManagement.Application.DTOs.Employee;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly WorkSphereDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public EmployeeService(
            WorkSphereDbContext context,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<PagedResult<EmployeeDto>> GetAllAsync(EmployeeQueryParameters parameters)
        {
            // 1. Temel sorguyu IQueryable olarak başlat — henüz hiçbir şey çalışmadı
            IQueryable<Employee> query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Where(e => !e.IsDeleted);

            // 2. Dinamik Arama — Ad, Soyad veya Email içinde ara
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(search) ||
                    e.LastName.ToLower().Contains(search) ||
                    e.Email.ToLower().Contains(search));
            }

            // 3. Departman Filtresi
            if (parameters.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == parameters.DepartmentId.Value);
            }

            // 4. Pozisyon Filtresi
            if (parameters.PositionId.HasValue)
            {
                query = query.Where(e => e.PositionId == parameters.PositionId.Value);
            }

            // 5. Aktiflik Filtresi
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == parameters.IsActive.Value);
            }

            // 6. Toplam kayıt sayısını AL — sayfalama öncesi, filtrelenmiş haliyle
            int totalCount = await query.CountAsync();

            // 7. Sayfalama — şimdi asıl SQL çalışıyor (OFFSET-FETCH ile)
            var employees = await query
                .AsNoTracking()
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

            return new PagedResult<EmployeeDto>
            {
                Items = employeeDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<EmployeeDto> GetByIdAsync(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (employee == null)
                throw new KeyNotFoundException($"{id} ID'li çalışan bulunamadı.");

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto createEmployeeDto)
        {
            var department = await _context.Departments.FindAsync(createEmployeeDto.DepartmentId);
            if (department == null)
                throw new KeyNotFoundException("Belirtilen departman bulunamadı.");

            var position = await _context.Positions.FindAsync(createEmployeeDto.PositionId);
            if (position == null)
                throw new KeyNotFoundException("Belirtilen pozisyon bulunamadı.");

            if (await _context.Employees.AnyAsync(e => e.Email == createEmployeeDto.Email))
                throw new InvalidOperationException("Bu e-posta adresine sahip bir çalışan zaten mevcut.");

            var baseUsername = createEmployeeDto.Email.Split('@')[0].ToLower();
            var username = baseUsername;
            int counter = 1;

            while (await _context.Users.AnyAsync(u => u.Username == username))
            {
                username = $"{baseUsername}{counter++}";
            }

            var user = new User
            {
                Username = username,
                PasswordHash = _passwordHasher.HashPassword("Employee123!"),
                IsActive = true
            };
            _context.Users.Add(user);

            var employeeRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");
            if (employeeRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    User = user,
                    RoleId = employeeRole.Id
                });
            }

            var employee = _mapper.Map<Employee>(createEmployeeDto);
            employee.User = user;
            employee.Department = department;
            employee.Position = position;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task UpdateAsync(Guid id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (employee == null)
                throw new KeyNotFoundException($"{id} ID'li çalışan bulunamadı.");

            var department = await _context.Departments.FindAsync(updateEmployeeDto.DepartmentId);
            if (department == null)
                throw new KeyNotFoundException("Belirtilen departman bulunamadı.");

            var position = await _context.Positions.FindAsync(updateEmployeeDto.PositionId);
            if (position == null)
                throw new KeyNotFoundException("Belirtilen pozisyon bulunamadı.");

            if (employee.Email != updateEmployeeDto.Email &&
                await _context.Employees.AnyAsync(e => e.Email == updateEmployeeDto.Email))
                throw new InvalidOperationException("Bu e-posta adresi başka bir çalışan tarafından kullanılıyor.");

            _mapper.Map(updateEmployeeDto, employee);
            employee.Department = department;
            employee.Position = position;
            employee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (employee == null)
                throw new KeyNotFoundException($"{id} ID'li çalışan bulunamadı.");

            employee.IsDeleted = true;
            employee.IsActive = false;
            employee.UpdatedDate = DateTime.UtcNow;

            if (employee.User != null)
            {
                employee.User.IsActive = false;
                employee.User.UpdatedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}