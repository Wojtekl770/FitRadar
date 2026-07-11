using FitRadar.Repositories.Interfaces;
using FitRadar.Shared.Models;
using FitRadar.Data;
using Microsoft.EntityFrameworkCore;

namespace FitRadar.Repositories
{
    public class EfProviderRepository : IProviderRepository
    {
        private readonly FitRadarDbContext _db;

        public EfProviderRepository(FitRadarDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Provider>> GetAllAsync (CancellationToken ct)
        {
            return await _db.Providers
                .Include(c => c.Packages)
                .ToListAsync(ct);
        }

        public async Task<Provider?> GetByIdAsync (Guid id, CancellationToken ct)
        {
            return await _db.Providers
                .Include(c => c.Packages)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task CreateAsync (Provider provider, CancellationToken ct)
        {
            await _db.Providers.AddAsync(provider, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync (Provider provider, CancellationToken ct)
        {
            _db.Providers.Update(provider);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync (Provider provider, CancellationToken ct)
        {
            _db.Providers.Remove(provider);
            await _db.SaveChangesAsync(ct);
        }
    }
}
