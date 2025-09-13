using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
        public class License
        {
            public int Id { get; set; }

            
            public string LicenseKey { get; set; } = null!;
            public string CustomerEmail { get; set; } = null!;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime ExpireAt { get; set; }
            public int MaxDevices { get; set; } = 1;
            public int UsedDevices { get; set; } = 0;
            public bool IsActive { get; set; } = true;

            
            public string? HostIdentifier { get; set; } // domain/machine id gibi
            public DateTime? RevokedAt { get; set; }    // lisans iptal edilirse
            public List<string> EnabledModules { get; set; } = new(); // hangi özellikleri açıyor
        }
    }
