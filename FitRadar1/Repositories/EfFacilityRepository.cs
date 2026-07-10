using FitRadar.Repositories.Interfaces;
using FitRadar.Shared.Models;
using FitRadar.Data;
using Microsoft.EntityFrameworkCore;


namespace FitRadar.Repositories
{
    public class EfFacilityRepository : IFacilityRepository
    {
        private readonly FitRadarDbContext _db;
        public EfFacilityRepository(FitRadarDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Facility>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Facilities
                .Include(c => c.Packages)
                .ToListAsync(ct);
        }
        public async Task<Facility?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Facilities
                .Include(c => c.Packages)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

        }
        public async Task CreateAsync(Facility facility, CancellationToken ct)
        {
            await _db.Facilities.AddAsync(facility, ct);
            await _db.SaveChangesAsync(ct);
        }
        public async Task UpdateAsync(Facility facility, CancellationToken ct)
        {
            _db.Facilities.Update(facility);
            await _db.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(Facility facility, CancellationToken ct)
        {
            _db.Facilities.Remove(facility);
            await _db.SaveChangesAsync(ct);
        }
        public async Task AssFacilityToPackage(Guid facilityId, Guid packageId, CancellationToken ct)
        {
            var facility = await _db.Facilities
                .FirstOrDefaultAsync(f => f.Id == facilityId, ct);

            var package = await _db.Packages
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == packageId, ct);

            if (package != null && facility != null && !package.Facilities.Contains(facility))
            {
                package.Facilities.Add(facility);
                await _db.SaveChangesAsync(ct);
            }
        }
        public async Task RemoveFacilityFromPackage(Guid facilityId, Guid packageId, CancellationToken ct)
        {
            var facility = await _db.Facilities
                .FirstOrDefaultAsync(f => f.Id == facilityId, ct);

            var package = await _db.Packages
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == packageId, ct);

            if (package != null && facility != null && package.Facilities.Contains(facility))
            {
                package.Facilities.Remove(facility);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}