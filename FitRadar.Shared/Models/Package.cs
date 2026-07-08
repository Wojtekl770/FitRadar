using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.Models
{
    public class Package
    {
        public Provider Provider { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal MonthlyPrice { get; set; } = 0;
        public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
        public Requirements Requirements { get; set; } = new Requirements();
    }
}
