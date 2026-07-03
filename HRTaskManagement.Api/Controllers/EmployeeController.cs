// HRTaskManagement.Api/Controllers/EmployeeController.cs
using HRTaskManagement.Application.DTOs.Employee;
using HRTaskManagement.Application.Interfaces; // Servis arayüzünüzün bulunduğu yer
using Microsoft.AspNetCore.Mvc;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // 1. GET ALL - Çalışan Listesini Getir
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees); // Geriye IEnumerable<EmployeeDto> dönecektir
        }

        // 2. GET BY ID - ID'ye Göre Çalışan Getir
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"{id} ID'li çalışan bulunamadı.");
            }
            return Ok(employee); // Geriye EmployeeDto dönecektir
        }

        // 3. CREATE - Yeni Çalışan Oluştur
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEmployee = await _employeeService.CreateAsync(createEmployeeDto);

            // HTTP 201 Created dönerken yeni oluşan kaynağın URL'ini ve kendisini teslim ediyoruz
            return CreatedAtAction(nameof(GetById), new { id = createdEmployee.Id }, createdEmployee);
        }

        // 4. UPDATE - Çalışan Güncelle
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _employeeService.UpdateAsync(id, updateEmployeeDto);
            if (!result)
            {
                return NotFound($"{id} ID'li çalışan bulunamadı veya güncellenemedi.");
            }

            return NoContent(); // HTTP 204 No Content güncelleme işlemleri için standarttır
        }

        // 5. DELETE - Çalışan Sil
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _employeeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"{id} ID'li çalışan bulunamadı.");
            }

            return NoContent(); // HTTP 204 No Content
        }
    }
}