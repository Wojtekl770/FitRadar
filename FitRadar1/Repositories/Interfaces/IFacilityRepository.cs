using FitRadar.Shared.Models;

namespace FitRadar.Repositories.Interfaces
{
    public interface IFacilityRepository
    {
        Task<IEnumerable<Facility>> GetAllAsync(CancellationToken ct);
        Task<Facility?> GetByIdAsync(Guid id, CancellationToken ct);
        Task CreateAsync(Facility facility, CancellationToken ct);
        Task UpdateAsync(Facility facility, CancellationToken ct);
        Task DeleteAsync(Facility facility, CancellationToken ct);
        Task AssFacilityToPackage(Guid facilityId, Guid packageId, CancellationToken ct);
        Task RemoveFacilityFromPackage(Guid facilityId, Guid packageId, CancellationToken ct);
    }
}
