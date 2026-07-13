// Api/Controllers/LeaveRequestController.cs
using HRTaskManagement.Application.DTOs.LeaveRequest;
using HRTaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ICurrentUserService _currentUserService;

        public LeaveRequestController(
            ILeaveRequestService leaveRequestService,
            ICurrentUserService currentUserService)
        {
            _leaveRequestService = leaveRequestService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] LeaveRequestQueryParameters queryParameters)
        {
            var result = await _leaveRequestService.GetAllAsync(queryParameters);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _leaveRequestService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            var result = await _leaveRequestService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Policy = "RequireManagerOrHROrAbove")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            var result = await _leaveRequestService.ApproveAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Policy = "RequireManagerOrHROrAbove")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            var result = await _leaveRequestService.RejectAsync(id, userId);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            var result = await _leaveRequestService.CancelAsync(id, userId);
            return Ok(result);
        }
    }
}
