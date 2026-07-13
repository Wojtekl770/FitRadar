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
        public Provider Provider { get; set; }
        public required string Name { get; set; }
        public decimal MonthlyPrice { get; set; }
        public List<Facility> Facilities { get; set; } = [];
        public bool OnlyForStudents { get; set; }
        public int EntriesPerMonth { get; set; }
        public List<User> Users { get; set; } = [];
    }
}
