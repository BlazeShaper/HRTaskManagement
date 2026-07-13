// Api/Controllers/AssetAssignmentController.cs
using HRTaskManagement.Application.DTOs.AssetAssignment;
using HRTaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetAssignmentController : ControllerBase
    {
        private readonly IAssetAssignmentService _assignmentService;
        private readonly ICurrentUserService _currentUserService;

        public AssetAssignmentController(
            IAssetAssignmentService assignmentService,
            ICurrentUserService currentUserService)
        {
            _assignmentService = assignmentService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] AssetAssignmentQueryParameters queryParameters)
        {
            var result = await _assignmentService.GetAllAsync(queryParameters);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _assignmentService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Assign([FromBody] CreateAssetAssignmentDto dto)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            var result = await _assignmentService.AssignAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id:guid}/return")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Return(Guid id, [FromBody] ReturnAssetDto dto)
        {
            var result = await _assignmentService.ReturnAsync(id, dto);
            return Ok(result);
        }
    }
}
