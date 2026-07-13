// Api/Controllers/TaskController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Filtreleme ve sayfalama parametrelerine göre tüm görevleri listeler.
        /// </summary>
        /// <param name="queryParameters">Arama terimi, öncelik, durum, çalışan ve sayfalama parametreleri.</param>
        /// <returns>Sayfalanmış görev listesi.</returns>
        /// <response code="200">Görev listesi başarıyla getirildi.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskQueryParameters queryParameters)
        {
            var tasks = await _taskService.GetAllAsync(queryParameters);
            return Ok(tasks);
        }

        /// <summary>
        /// Benzersiz kimliği ile tek bir görevin detaylarını getirir.
        /// </summary>
        /// <param name="id">Görevin Guid kimliği.</param>
        /// <returns>Görev detay bilgileri.</returns>
        /// <response code="200">Görev başarıyla bulundu ve getirildi.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="404">Görev bulunamadı.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            return Ok(task);
        }

        /// <summary>
        /// Yeni bir iş görevi oluşturur.
        /// </summary>
        /// <param name="request">Oluşturulacak görevin başlık, açıklama, öncelik ve atanacak kişi bilgileri.</param>
        /// <returns>Yeni oluşturulan görevin detayları.</returns>
        /// <response code="201">Görev başarıyla oluşturuldu.</response>
        /// <response code="400">Verilen veriler geçersiz.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="403">Bu işlem için gerekli olan yetkiye sahip değilsiniz (Manager ve üzeri).</response>
        [HttpPost]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto request)
        {
            var result = await _taskService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Belirtilen kimliğe sahip görevin tüm detaylarını günceller.
        /// </summary>
        /// <param name="id">Görevin Guid kimliği.</param>
        /// <param name="request">Güncellenecek görev bilgileri.</param>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Görev başarıyla güncellendi.</response>
        /// <response code="400">Verilen veriler geçersiz veya kimlik eşleşmiyor.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="403">Bu işlem için gerekli olan yetkiye sahip değilsiniz (Manager ve üzeri).</response>
        /// <response code="404">Görev bulunamadı.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto request)
        {
            await _taskService.UpdateAsync(id, request);
            return NoContent();
        }

        /// <summary>
        /// Belirtilen kimliğe sahip görevi siler.
        /// </summary>
        /// <param name="id">Görevin Guid kimliği.</param>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Görev başarıyla silindi.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="403">Bu işlem için gerekli olan yetkiye sahip değilsiniz (Manager ve üzeri).</response>
        /// <response code="404">Görev bulunamadı.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "RequireManagerOrAbove")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _taskService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Görevin durumunu (Örn: Yapılacak, Devam Ediyor, Tamamlandı) günceller.
        /// </summary>
        /// <param name="id">Görevin Guid kimliği.</param>
        /// <param name="request">Görevin yeni durum değeri.</param>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Görev durumu başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen veri geçersiz veya kimlik eşleşmiyor.</response>
        /// <response code="401">Yetkisiz erişim.</response>
        /// <response code="404">Görev bulunamadı.</response>
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusDto request)
        {
            await _taskService.UpdateStatusAsync(id, request);
            return NoContent();
        }
    }
}