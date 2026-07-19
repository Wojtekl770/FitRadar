using FitRadar.Data;
using FitRadar.Repositories.Interfaces;
using FitRadar.Shared.Models;
using Microsoft.EntityFrameworkCore;


namespace FitRadar.Repositories
{
    public class EfPackageRepository : IPackageRepository
    {
        private readonly FitRadarDbContext _db;

        public EfPackageRepository(FitRadarDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Package>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Packages
                .Include(p => p.Facilities)
                .ToListAsync(ct);
        }

        public async Task<Package?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Packages
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IEnumerable<Package?>> GetByProviderAsync(Guid providerId, CancellationToken ct)
        {
            return await _db.Packages
                .Include(p => p.Facilities)
                .Where(p => p.ProviderId == providerId)
                .ToListAsync(ct);
        }

        public async Task CreateAsync(Package package, CancellationToken ct)
        {
            await _db.Packages.AddAsync(package, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Package package, CancellationToken ct)
        {
            _db.Packages.Update(package);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid packageId, CancellationToken ct)
        {
            var package = await _db.Packages.FirstOrDefaultAsync(p => p.Id == packageId, ct);
            _db.Packages.Remove(package);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddFacilityToPackageAsync(Guid facilityId, Guid packageId, CancellationToken ct)
        {
            var facility = await _db.Facilities
                .FirstOrDefaultAsync(f => f.Id == facilityId, ct);
            var package = await _db.Packages
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == packageId, ct);

            if (package != null && facility != null && !package.Facilities.Contains(facility))
            {
                package.Facilities.Add(facility);
                facility.Packages.Add(package);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task RemoveFacilityFromPackageAsync(Guid facilityId, Guid packageId, CancellationToken ct)
        {
            var facility = await _db.Facilities
                .FirstOrDefaultAsync(f => f.Id == facilityId, ct);
            var package = await _db.Packages
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == packageId, ct);

            if (package != null && facility != null && package.Facilities.Contains(facility))
            {
                package.Facilities.Remove(facility);
                facility.Packages.Remove(package);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task AddUserToPackageAsync(Guid userId, Guid packageId, CancellationToken ct)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            var package = await _db.Packages
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == packageId, ct);

            if (package != null && user != null && !package.Users.Contains(user))
            {
                package.Users.Add(user);
                await _db.SaveChangesAsync(ct);
            }
        }
        public async Task RemoveUserFromPackageAsync(Guid userId, Guid packageId, CancellationToken ct)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            var package = await _db.Packages
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == packageId, ct);

            if (package != null && user != null && !package.Users.Contains(user))
            {
                package.Users.Remove(user);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
