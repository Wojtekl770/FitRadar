using FitRadar.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs
{
    public class PackageDto
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal MonthlyPrice { get; set; }
        public bool OnlyForStudents { get; set; }
        public int EntriesPerMonth { get; set; }
        public List<Guid> FacilityIds { get; set; } = [];
    }

    // Jeśli klient potrzebuje pełnych detali Facilities
    public class PackageDetailsDto
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal MonthlyPrice { get; set; }
        public bool OnlyForStudents { get; set; }
        public int EntriesPerMonth { get; set; }
        public List<FacilitySummaryDto> Facilities { get; set; } = [];
    }

    public class PackageSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal MonthlyPrice { get; set; }
    }
}
