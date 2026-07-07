// Api/Controllers/TaskController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Task;
using HRTaskManagement.Application.Interfaces;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskQueryParameters queryParameters)
        {
            var tasks = await _taskService.GetAllAsync(queryParameters);
            return Ok(tasks);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return Ok(task);
        }

        [HttpPost]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto request)
        {
            var result = await _taskService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto request)
        {
            await _taskService.UpdateAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusDto request)
        {
            await _taskService.UpdateStatusAsync(id, request);
            return NoContent();
        }
    }
}