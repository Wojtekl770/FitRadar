using FitRadar.Shared.Models;

namespace FitRadar.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        Task<IEnumerable<Package>> GetAllAsync(CancellationToken ct);
        Task<Package?> GetByIdAsync(Guid id, CancellationToken ct);
        Task CreateAsync(Package package, CancellationToken ct);
        Task UpdateAsync(Package package, CancellationToken ct);
        Task DeleteAsync(Package package, CancellationToken ct);
        Task AddFacilityToPackage(Guid facilityId, Guid packageId, CancellationToken ct);
        Task RemoveFacilityFromPackage(Guid facilityId, Guid packageId, CancellationToken ct);
        Task AddUserToPackage(Guid userId, Guid packageId, CancellationToken ct);
        Task RemoveUserFromPackage(Guid userId, Guid packageId, CancellationToken ct);
    }
}
