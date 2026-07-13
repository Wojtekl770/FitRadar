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
        public async Task<IEnumerable<Facility>> GetByTypeAsync(Shared.Models.Type type, CancellationToken ct)
        {
            return await _db.Facilities
                .Include(c => c.Packages)
                .Where(c => c.Type == type)
                .ToListAsync(ct);
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
        public async Task DeleteAsync(Guid facilityid, CancellationToken ct)
        {
            var facility = await _db.Facilities.FirstOrDefaultAsync(c => c.Id == facilityid, ct);
            _db.Facilities.Remove(facility);
            await _db.SaveChangesAsync(ct);
        }
    }
}