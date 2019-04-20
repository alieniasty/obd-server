using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace OBDPI.Server.Data.Models
{
    public class User : IdentityUser
    {
        public bool IsVerified { get; set; } = false;
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
