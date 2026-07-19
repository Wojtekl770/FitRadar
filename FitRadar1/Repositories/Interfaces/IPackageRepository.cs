using FitRadar.Shared.Models;

namespace FitRadar.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        Task<IEnumerable<Package>> GetAllAsync(CancellationToken ct);
        Task<Package?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<Package?>> GetByProviderAsync(Guid providerId, CancellationToken ct);
        Task CreateAsync(Package package, CancellationToken ct);
        Task UpdateAsync(Package package, CancellationToken ct);
        Task DeleteAsync(Guid packageId, CancellationToken ct);
        Task AddFacilityToPackageAsync(Guid facilityId, Guid packageId, CancellationToken ct);
        Task RemoveFacilityFromPackageAsync(Guid facilityId, Guid packageId, CancellationToken ct);
        Task AddUserToPackageAsync(Guid userId, Guid packageId, CancellationToken ct);
        Task RemoveUserFromPackageAsync(Guid userId, Guid packageId, CancellationToken ct);
    }
}
