using FitRadar.Shared.Models;

namespace FitRadar.Repositories.Interfaces
{
    public interface IFacilityRepository
    {
        Task<IEnumerable<Facility>> GetAllAsync(CancellationToken ct);
        Task<Facility?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<Facility>> GetByTypeAsync(Shared.Models.Type type, CancellationToken ct);
        Task CreateAsync(Facility facility, CancellationToken ct);
        Task UpdateAsync(Facility facility, CancellationToken ct);
        Task DeleteAsync(Guid facilityId, CancellationToken ct);
    }
}
