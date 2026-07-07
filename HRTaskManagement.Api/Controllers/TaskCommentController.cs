// Api/Controllers/TaskCommentController.cs
using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.TaskComment;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskCommentController : ControllerBase
    {
        private readonly ITaskCommentService _commentService;
        private readonly ICurrentUserService _currentUserService;

        public TaskCommentController(
            ITaskCommentService commentService,
            ICurrentUserService currentUserService)
        {
            _commentService = commentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] TaskCommentQueryParameters queryParameters)
        {
            var result = await _commentService.GetAllAsync(queryParameters);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _commentService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskCommentDto dto)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            var result = await _commentService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            var isManagerOrAbove = _currentUserService.IsInRole(SystemRoles.Admin)
                || _currentUserService.IsInRole(SystemRoles.Manager);

            await _commentService.DeleteAsync(id, userId, isManagerOrAbove);
            return NoContent();
        }
    }
}
