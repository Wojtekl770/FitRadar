using FitRadar.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitRadar.Shared.DTOs
{
    public enum Type
    {
        Gym,
        Swimming_Pool,
        Fitness_Club
    }
    public class FacilityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public Type Type { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public List<Package> Packages { get; set; } = [];
    }
}
