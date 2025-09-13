using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LisanceDTO
{
    public class GenerateLicenseRequest
    {
        public string Email { get; set; } = null!;
        public int DurationDays { get; set; }
        public int MaxDevices { get; set; }
    }
}
