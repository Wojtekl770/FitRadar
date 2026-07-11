using FitRadar.Shared.Models;

namespace FitRadar.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task CreateAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user, CancellationToken ct);
        Task DeleteAsync(User user, CancellationToken ct);
        Task<bool?> IsUserStudent(Guid userId, CancellationToken ct);
    }
}
