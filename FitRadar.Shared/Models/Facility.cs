using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.Models
{
    public class Facility
    {
        public string Name { get; set; } = null!;
        public string Adress { get; set; } = null!;
        public float latitude { get; set; } = 0;
        public float longitude { get; set; } = 0;
    }
}
