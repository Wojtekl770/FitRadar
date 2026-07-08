using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.Models
{
    public class Package
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Provider Provider { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal MonthlyPrice { get; set; } = 0;
        public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
        public bool OnlyForStudents { get; set; } = false;
        public int EntriesPerMonth { get; set; } = 0;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
