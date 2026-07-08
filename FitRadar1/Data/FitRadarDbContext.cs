using FitRadar.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitRadar.Data
{
    public class FitRadarDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public FitRadarDbContext(DbContextOptions<FitRadarDbContext> options) 
            : base(options) { }

        public DbSet<Package> Packages { get; set; } = null!;
        public DbSet<Facility> Facilities { get; set; } = null!;
        public DbSet<Provider> Providers { get; set; } = null!;
        public DbSet<Requirements> Requirements { get; set; } = null!;
    }
}
