using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AppUser : IdentityUser
    {
        // Lisanslama için müşteri bilgilerini ekleyebilirsin
        public string? CompanyName { get; set; }
    }
}
