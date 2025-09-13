using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LisanceDTO
{
    public class IssueLicenseRequest
    {
        public string? Key { get; set; }  // boş bırakılırsa backend generate edecek
        public string? HostIdentifier { get; set; } // domain/machine id
        public DateTime? ExpireAt { get; set; }
        public IEnumerable<string>? EnabledModules { get; set; }
        public string? CustomerEmail { get; set; }
    }
}
