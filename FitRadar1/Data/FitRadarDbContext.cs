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
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;   // <-- dodane

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User <-> Packages (many-to-many)
            builder.Entity<User>()
                .HasMany(p => p.Packages)
                .WithMany(u => u.Users)
                .UsingEntity(j => j.HasData());

            // Package <-> Facilites (many-to-many)
            builder.Entity<Package>()
                .HasMany(f => f.Facilities)
                .WithMany(p => p.Packages)
                .UsingEntity(j => j.HasData());

            // Provider -> Package (one-to-many)
            builder.Entity<Package>()
                .HasOne(p => p.Provider)
                .WithMany(pr => pr.Packages)
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Package>()
                .Property(p => p.MonthlyPrice)
                .HasPrecision(18, 2);

            // User -> RefreshTokens (one-to-many)               // <-- dodane
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rt => rt.Token).IsUnique();
            });
        }
    }
}