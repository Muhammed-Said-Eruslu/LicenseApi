using Application.DTOs.LisanceDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.LisanceService
{
    public interface ILicenseService
    {
        Task<(bool IsValid, string Message, DateTime? ExpireDate)> ValidateLicenseAsync(string key, string machineId);
        Task<string> GenerateLicenseAsync(string email, int durationDays, int maxDevices);
        Task<List<License>> GetAllAsync();
        Task<string> IssueLicenseAsync(IssueLicenseRequest req);
        Task<bool> RevokeAsync(string licenseKey);
    }
}
