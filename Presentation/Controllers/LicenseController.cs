using Application.DTOs.LisanceDTO;
using Application.Interfaces.LisanceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        // Lisans doğrulama
        [HttpPost("validate")]
        [AllowAnonymous] // istemci uygulamalar (örn. OtoArac) token olmadan doğrular
        public async Task<IActionResult> Validate([FromBody] LicenseCheckRequest req)
        {
             var result = await _licenseService.ValidateLicenseAsync(req.LicenseKey, req.MachineId);
            if (!result.IsValid)
                return Unauthorized(new { result.Message });

            return Ok(new { result.Message, result.ExpireDate });
        }

        // Lisans oluşturma (kullanıcı için)
        [HttpPost("generate")]
        [Authorize(Roles = "ADMIN")] // sadece admin kullanabilir
        public async Task<IActionResult> Generate([FromBody] GenerateLicenseRequest req)
        {
            var key = await _licenseService.GenerateLicenseAsync(req.Email, req.DurationDays, req.MaxDevices);
            return Ok(new { LicenseKey = key });
        }

        // Bazı istemciler GET isteği gönderdiğinde 404 almaması için
        [HttpGet("generate")]
        [Authorize(Roles = "ADMIN")] // sadece admin kullanabilir
        public async Task<IActionResult> GenerateFromQuery(
            [FromQuery] string email,
            [FromQuery] int durationDays,
            [FromQuery] int maxDevices)
        {
            var key = await _licenseService.GenerateLicenseAsync(email, durationDays, maxDevices);
            return Ok(new { LicenseKey = key });
        }

        // Admin: yeni lisans üretme (gelişmiş)
        [HttpPost("issue")]
        [Authorize(Roles = "ADMIN")] // sadece admin kullanabilir
        public async Task<IActionResult> Issue([FromBody] IssueLicenseRequest req)
        {
            var key = await _licenseService.IssueLicenseAsync(req);
            return Ok(new { LicenseKey = key });
        }

        // Admin: lisansı iptal etme
        [HttpPost("revoke")]
        [Authorize(Roles = "ADMIN")] // sadece admin kullanabilir
        public async Task<IActionResult> Revoke([FromBody] RevokeRequest req)
        {
            var ok = await _licenseService.RevokeAsync(req.LicenseKey);
            if (!ok) return NotFound(new { Message = "Lisans bulunamadı." });

            return Ok(new { Message = "Lisans iptal edildi." });
        }

        // Tüm lisansları listeleme
        [HttpGet]
        [Authorize(Roles = "ADMIN")] // sadece admin kullanabilir
        public async Task<IActionResult> GetAll()
        {
            var licenses = await _licenseService.GetAllAsync();
            return Ok(licenses);
        }
    }
}
