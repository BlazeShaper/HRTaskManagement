// Api/Controllers/EmployeeController.cs
using HRTaskManagement.Application.DTOs.Employee;
using HRTaskManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Filtreleme ve sayfalama parametrelerine göre çalışan listesini getirir.
        /// </summary>
        /// <param name="parameters">Arama terimi, departman, pozisyon, aktiflik durumu ve sayfalama parametreleri.</param>
        /// <returns>Sayfalanmış çalışan listesi.</returns>
        /// <response code="200">Çalışan listesi başarıyla getirildi.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] EmployeeQueryParameters parameters)
        {
            var employees = await _employeeService.GetAllAsync(parameters);
            return Ok(employees);
        }

        /// <summary>
        /// Belirtilen benzersiz kimliğe sahip çalışanın detaylarını getirir.
        /// </summary>
        /// <param name="id">Çalışanın benzersiz Guid kimliği.</param>
        /// <returns>Çalışan detay bilgileri.</returns>
        /// <response code="200">Çalışan bilgisi başarıyla getirildi.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="404">Belirtilen kimliğe sahip çalışan bulunamadı.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return Ok(employee);
        }

        /// <summary>
        /// Sistemde yeni bir çalışan kaydı ve buna bağlı kullanıcı hesabı oluşturur.
        /// </summary>
        /// <param name="createEmployeeDto">Yeni çalışanın kişisel, iletişim ve departman/pozisyon bilgileri.</param>
        /// <returns>Oluşturulan çalışanın bilgileri ve sistem tarafından üretilen geçici şifresi.</returns>
        /// <response code="201">Çalışan başarıyla oluşturuldu.</response>
        /// <response code="400">Gönderilen veriler geçersiz.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="403">Bu işlem için gerekli olan yetki rolüne sahip değilsiniz (Admin veya HR).</response>
        [HttpPost]
        [Authorize(Policy = "AdminOrHR")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            var result = await _employeeService.CreateAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Employee.Id }, result);
        }

        /// <summary>
        /// Belirtilen kimliğe sahip çalışanın bilgilerini günceller.
        /// </summary>
        /// <param name="id">Çalışanın benzersiz Guid kimliği.</param>
        /// <param name="updateEmployeeDto">Güncellenecek çalışan bilgileri.</param>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Güncelleme başarıyla tamamlandı.</response>
        /// <response code="400">Gönderilen veriler geçersiz veya kimlik eşleşmiyor.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="403">Bu işlem için gerekli yetkiye sahip değilsiniz (Manager ve üzeri).</response>
        /// <response code="404">Çalışan bulunamadı.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            await _employeeService.UpdateAsync(id, updateEmployeeDto);
            return NoContent();
        }

        /// <summary>
        /// Belirtilen kimliğe sahip çalışanı sistemden yumuşak silme (soft-delete) yöntemiyle siler.
        /// </summary>
        /// <param name="id">Silinecek çalışanın benzersiz Guid kimliği.</param>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Çalışan başarıyla silindi.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="403">Bu işlem için gerekli olan yetki rolüne sahip değilsiniz (Admin veya HR).</response>
        /// <response code="404">Çalışan bulunamadı.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "AdminOrHR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}