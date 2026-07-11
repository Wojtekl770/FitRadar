using FitRadar.Repositories.Interfaces;
using FitRadar.Shared.Models;
using FitRadar.Data;
using Microsoft.EntityFrameworkCore;

namespace FitRadar.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly FitRadarDbContext _db;
        public EfUserRepository(FitRadarDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Users.ToListAsync(ct);
        }
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Users
                .Include(p => p.Packages)
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }
        public async Task CreateAsync(User user, CancellationToken ct)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }
        public async Task UpdateAsync(User user, CancellationToken ct)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
        }
        public async Task DeleteAsync(User user, CancellationToken ct)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool?> IsUserStudent(Guid userId, CancellationToken ct)
        {
            var user = await GetByIdAsync(userId, ct);
            if (user != null)
            {
                bool isStudent = user.IsStudent;
                return isStudent;
            }
            return null;
        }
    }
}
