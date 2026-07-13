using FitRadar.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs
{
    public class FacilityInputDto
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public Models.Type Type { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}
