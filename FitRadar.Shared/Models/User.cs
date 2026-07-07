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
    }
}
