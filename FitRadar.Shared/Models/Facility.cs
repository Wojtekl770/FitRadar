using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.Models
{
    public enum Type
    {
        Gym,
        Swimming_Pool,
        Fitness_Club
    }
    public class Facility
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public Type Type { get; set; } = Type.Gym;
        public float Latitude { get; set; } = 0;
        public float Longitude { get; set; } = 0;
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
