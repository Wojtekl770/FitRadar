using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FitRadar.Shared.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsStudent { get; set; } = false;
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
