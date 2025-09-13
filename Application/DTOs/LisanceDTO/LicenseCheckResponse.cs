using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LisanceDTO
{
    public class LicenseCheckResponse
    {
        public bool Valid { get; set; }
        public string? Reason { get; set; }
        public List<string> EnabledModules { get; set; } = new();

        public static LicenseCheckResponse ValidResponse(List<string>? modules = null)
        {
            return new LicenseCheckResponse
            {
                Valid = true,
                EnabledModules = modules ?? new List<string>()
            };
        }

        public static LicenseCheckResponse InvalidResponse(string reason)
        {
            return new LicenseCheckResponse
            {
                Valid = false,
                Reason = reason,
                EnabledModules = new List<string>()
            };
        }
    }
}
