// Persistence/Services/LeaveRequestService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.LeaveRequest;
using HRTaskManagement.Application.DTOs.Notification;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Domain.Enums;
using HRTaskManagement.Persistence.Context;
using HRTaskManagement.Shared.Constants;

namespace HRTaskManagement.Persistence.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly WorkSphereDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public LeaveRequestService(
            WorkSphereDbContext dbContext,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        private static IQueryable<LeaveRequestDto> ProjectToDto(
            IQueryable<LeaveRequest> source)
        {
            return source.Select(lr => new LeaveRequestDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeFullName = lr.Employee!.FirstName + " " + lr.Employee.LastName,
                LeaveType = lr.LeaveType.ToString(),
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Status = lr.Status.ToString(),
                Reason = lr.Reason,
                ApprovedBy = lr.ApprovedBy,
                ApprovedByUsername = lr.ApprovedByUser != null ? lr.ApprovedByUser.Username : null,
                CreatedDate = lr.CreatedDate
            });
        }

        public async Task<PagedResult<LeaveRequestDto>> GetAllAsync(
            LeaveRequestQueryParameters queryParameters)
        {
            var query = _dbContext.LeaveRequests
                .AsNoTracking()
                .Where(lr => !lr.IsDeleted);

            if (queryParameters.EmployeeId.HasValue)
                query = query.Where(lr => lr.EmployeeId == queryParameters.EmployeeId.Value);

            if (!string.IsNullOrWhiteSpace(queryParameters.Status) &&
                Enum.TryParse<LeaveStatus>(queryParameters.Status, true, out var statusFilter))
                query = query.Where(lr => lr.Status == statusFilter);

            if (!string.IsNullOrWhiteSpace(queryParameters.LeaveType) &&
                Enum.TryParse<LeaveType>(queryParameters.LeaveType, true, out var typeFilter))
                query = query.Where(lr => lr.LeaveType == typeFilter);

            var totalCount = await query.CountAsync();

            var items = await ProjectToDto(query)
                .OrderByDescending(lr => lr.CreatedDate)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            return new PagedResult<LeaveRequestDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }

        public async Task<LeaveRequestDto> GetByIdAsync(Guid id)
        {
            var query = _dbContext.LeaveRequests
                .AsNoTracking()
                .Where(lr => lr.Id == id && !lr.IsDeleted);

            var result = await ProjectToDto(query).FirstOrDefaultAsync();
            return result ?? throw new KeyNotFoundException("İzin talebi bulunamadı.");
        }

        public async Task<LeaveRequestDto> CreateAsync(
            CreateLeaveRequestDto dto, Guid currentUserId)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.UserId == currentUserId && !e.IsDeleted)
                ?? throw new InvalidOperationException("Bu kullanıcıya bağlı bir çalışan kaydı bulunamadı.");

            if (!Enum.TryParse<LeaveType>(dto.LeaveType, true, out var leaveType))
                throw new InvalidOperationException("Geçersiz izin türü.");

            // Çakışan tarih kontrolü
            var overlapping = await _dbContext.LeaveRequests.AnyAsync(lr =>
                lr.EmployeeId == employee.Id &&
                !lr.IsDeleted &&
                lr.Status != LeaveStatus.Rejected &&
                lr.Status != LeaveStatus.Cancelled &&
                lr.StartDate <= dto.EndDate &&
                lr.EndDate >= dto.StartDate);

            if (overlapping)
                throw new InvalidOperationException("Bu tarih aralığında zaten bir izin talebiniz bulunuyor.");

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employee.Id,
                LeaveType = leaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = LeaveStatus.Pending
            };

            _dbContext.LeaveRequests.Add(leaveRequest);
            await _dbContext.SaveChangesAsync();

            // Yöneticilere/Admin'lere bildirim gönder
            await NotifyManagersAboutNewLeaveRequest(leaveRequest, employee);

            return await GetByIdAsync(leaveRequest.Id);
        }

        public async Task<LeaveRequestDto> ApproveAsync(Guid id, Guid approverUserId)
        {
            var leaveRequest = await _dbContext.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && !lr.IsDeleted)
                ?? throw new KeyNotFoundException("İzin talebi bulunamadı.");

            if (leaveRequest.Status != LeaveStatus.Pending)
                throw new InvalidOperationException(
                    $"Yalnızca 'Pending' durumundaki talepler onaylanabilir. Mevcut durum: {leaveRequest.Status}");

            leaveRequest.Status = LeaveStatus.Approved;
            leaveRequest.ApprovedBy = approverUserId;
            leaveRequest.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            // Onay bildirimi gönder
            await SendLeaveStatusNotification(leaveRequest);

            return await GetByIdAsync(id);
        }

        public async Task<LeaveRequestDto> RejectAsync(Guid id, Guid approverUserId)
        {
            var leaveRequest = await _dbContext.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && !lr.IsDeleted)
                ?? throw new KeyNotFoundException("İzin talebi bulunamadı.");

            if (leaveRequest.Status != LeaveStatus.Pending)
                throw new InvalidOperationException(
                    $"Yalnızca 'Pending' durumundaki talepler reddedilebilir. Mevcut durum: {leaveRequest.Status}");

            leaveRequest.Status = LeaveStatus.Rejected;
            leaveRequest.ApprovedBy = approverUserId;
            leaveRequest.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            // Red bildirimi gönder
            await SendLeaveStatusNotification(leaveRequest);

            return await GetByIdAsync(id);
        }

        public async Task<LeaveRequestDto> CancelAsync(Guid id, Guid currentUserId)
        {
            var leaveRequest = await _dbContext.LeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && !lr.IsDeleted)
                ?? throw new KeyNotFoundException("İzin talebi bulunamadı.");

            // Sadece talep sahibi iptal edebilir
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.UserId == currentUserId && !e.IsDeleted)
                ?? throw new InvalidOperationException("Çalışan kaydı bulunamadı.");

            if (leaveRequest.EmployeeId != employee.Id)
                throw new UnauthorizedAccessException("Sadece kendi izin talebinizi iptal edebilirsiniz.");

            if (leaveRequest.Status != LeaveStatus.Pending)
                throw new InvalidOperationException(
                    $"Yalnızca 'Pending' durumundaki talepler iptal edilebilir. Mevcut durum: {leaveRequest.Status}");

            leaveRequest.Status = LeaveStatus.Cancelled;
            leaveRequest.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // ============================================
        // PRIVATE HELPERS — Bildirim metodları
        // ============================================

        private async Task SendLeaveStatusNotification(LeaveRequest leaveRequest)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == leaveRequest.EmployeeId && !e.IsDeleted);

            if (employee?.UserId is Guid employeeUserId)
            {
                if (leaveRequest.Status == LeaveStatus.Approved)
                {
                    await _notificationService.CreateAsync(new CreateNotificationDto
                    {
                        UserId = employeeUserId,
                        Title = "İzin Talebiniz Onaylandı",
                        Message = $"{leaveRequest.StartDate:dd.MM.yyyy} - {leaveRequest.EndDate:dd.MM.yyyy} tarihleri arasındaki izin talebiniz onaylandı.",
                        Type = NotificationType.LeaveRequestApproved.ToString()
                    });
                }
                else if (leaveRequest.Status == LeaveStatus.Rejected)
                {
                    await _notificationService.CreateAsync(new CreateNotificationDto
                    {
                        UserId = employeeUserId,
                        Title = "İzin Talebiniz Reddedildi",
                        Message = $"{leaveRequest.StartDate:dd.MM.yyyy} - {leaveRequest.EndDate:dd.MM.yyyy} tarihleri arasındaki izin talebiniz reddedildi.",
                        Type = NotificationType.LeaveRequestRejected.ToString()
                    });
                }
            }
        }

        private async Task NotifyManagersAboutNewLeaveRequest(LeaveRequest leaveRequest, Employee employee)
        {
            // Admin ve Manager rolündeki tüm kullanıcılara bildirim gönder
            var managerUserIds = await _dbContext.Users
                .Where(u => !u.IsDeleted && u.IsActive)
                .Join(_dbContext.UserRoles.Where(ur =>
                    ur.Role!.Name == SystemRoles.Admin || ur.Role!.Name == SystemRoles.Manager),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => user.Id)
                .Distinct()
                .ToListAsync();

            foreach (var managerUserId in managerUserIds)
            {
                await _notificationService.CreateAsync(new CreateNotificationDto
                {
                    UserId = managerUserId,
                    Title = "Yeni İzin Talebi",
                    Message = $"{employee.FirstName} {employee.LastName} tarafından {leaveRequest.StartDate:dd.MM.yyyy} - {leaveRequest.EndDate:dd.MM.yyyy} tarihleri için yeni bir izin talebi oluşturuldu.",
                    Type = NotificationType.LeaveRequestSubmitted.ToString()
                });
            }
        }
    }
}