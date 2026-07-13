using FitRadar.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs
{
    public class ProviderDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<Package> Packages { get; set; } = [];
    }
}
