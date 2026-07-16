using FitRadar.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs
{
    public class FacilityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Models.Type Type { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public List<Guid> PackageIds { get; set; } = [];
    }

    public class FacilitySummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Models.Type Type { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class FacilityDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Models.Type Type { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public List<PackageSummaryDto> Packages { get; set; } = [];
    }

    public class FacilityInputDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Models.Type Type { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
