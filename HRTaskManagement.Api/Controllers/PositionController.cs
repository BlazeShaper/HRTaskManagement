using HRTaskManagement.Application.DTOs.Position;
using HRTaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PositionQueryParameters queryParameters)
        {
            var result = await _positionService.GetAllAsync(queryParameters);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _positionService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Create([FromBody] CreatePositionDto dto)
        {
            var result = await _positionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionDto dto)
        {
            var result = await _positionService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _positionService.DeleteAsync(id);
            return NoContent();
        }
    }
}