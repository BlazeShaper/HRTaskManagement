// Api/Controllers/AssetController.cs
using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Asset;
using HRTaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AssetQueryParameters queryParameters)
        {
            var result = await _assetService.GetAllAsync(queryParameters);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _assetService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Create([FromBody] CreateAssetDto dto)
        {
            var result = await _assetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetDto dto)
        {
            var result = await _assetService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _assetService.DeleteAsync(id);
            return NoContent();
        }
    }
}
