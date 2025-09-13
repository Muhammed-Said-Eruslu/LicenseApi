using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs.LisanceDTO
{
    public class LicenseCheckRequest
    {
        [JsonPropertyName("licenseKey")]
        public string LicenseKey { get; set; } = null!;

        [JsonPropertyName("machineId")]
        public string MachineId { get; set; } = null!;
    }
}
