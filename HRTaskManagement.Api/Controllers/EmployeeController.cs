// Api/Controllers/EmployeeController.cs
using HRTaskManagement.Application.DTOs.Employee;
using HRTaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] EmployeeQueryParameters parameters)
        {
            var employees = await _employeeService.GetAllAsync(parameters);
            return Ok(employees);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrHR")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            var result = await _employeeService.CreateAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Employee.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            await _employeeService.UpdateAsync(id, updateEmployeeDto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOrHR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}