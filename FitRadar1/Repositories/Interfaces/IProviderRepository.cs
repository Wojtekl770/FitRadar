using FitRadar.Shared.Models;

namespace FitRadar.Repositories.Interfaces
{
    public interface IProviderRepository
    {
        Task<IEnumerable<Provider>> GetAllAsync(CancellationToken ct);
        Task<Provider?> GetByIdAsync(Guid id, CancellationToken ct);
        Task CreateAsync(Provider provider, CancellationToken ct);
        Task UpdateAsync(Provider provider, CancellationToken ct);
        Task DeleteAsync(Provider provider, CancellationToken ct);
    }
}
