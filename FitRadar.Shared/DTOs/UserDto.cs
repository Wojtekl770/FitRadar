using FitRadar.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsStudent { get; set; }
        public List<Package> Packages { get; set; } = [];
    }
}
