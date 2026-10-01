using Application.DTOs.LisanceDTO;
using Application.Interfaces.LisanceService;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly AppDbContext _context;

        public LicenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateLicenseAsync(string email, int durationDays, int maxDevices)
        {
            string key = Guid.NewGuid().ToString("N").ToUpper();

            var license = new License
            {
                LicenseKey = key,
                CustomerEmail = email,
                ExpireAt = DateTime.UtcNow.AddDays(durationDays),
                MaxDevices = maxDevices,
                IsActive = true
            };

            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();

            return key;
        }

        public async Task<(bool IsValid, string Message, DateTime? ExpireDate)> ValidateLicenseAsync(string key, string machineId)
        {
            var license = await _context.Licenses.FirstOrDefaultAsync(x => x.LicenseKey == key);

            if (license == null)
                return (false, "Lisans bulunamadı.", null);

            if (!license.IsActive)
                return (false, "Lisans pasif durumda.", null);

            if (license.ExpireAt < DateTime.UtcNow)
                return (false, "Lisans süresi dolmuş.", license.ExpireAt);

            if (license.UsedDevices >= license.MaxDevices)
                return (false, "Maksimum cihaz limitine ulaşıldı.", license.ExpireAt);

            // Lisans belirli bir makineye/domaine bağlıysa başka bir cihazdan kullanılamaz
            if (!string.IsNullOrWhiteSpace(license.HostIdentifier) && license.HostIdentifier != machineId)
                return (false, "Lisans bu cihaz için geçerli değil.", license.ExpireAt);

            return (true, "Lisans geçerli.", license.ExpireAt);
        }

        public async Task<List<License>> GetAllAsync()
        {
            return await _context.Licenses.ToListAsync();
        }

        public async Task<string> IssueLicenseAsync(IssueLicenseRequest req)
        {
            string key = string.IsNullOrWhiteSpace(req.Key) ? Guid.NewGuid().ToString("N") : req.Key;

            var license = new License
            {
                LicenseKey = key,
                CustomerEmail = req.CustomerEmail ?? "unknown@test.com", // NULL olmasın diye
                HostIdentifier = req.HostIdentifier,
                ExpireAt = req.ExpireAt ?? DateTime.UtcNow.AddDays(30),
                IsActive = true,
                EnabledModules = req.EnabledModules?.ToList() ?? new()
            };
            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();
            return key;
        }

        public async Task<bool> RevokeAsync(string licenseKey)
        {
            var license = await _context.Licenses.FirstOrDefaultAsync(x => x.LicenseKey == licenseKey);
            if (license == null) return false;

            license.IsActive = false;
            license.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
