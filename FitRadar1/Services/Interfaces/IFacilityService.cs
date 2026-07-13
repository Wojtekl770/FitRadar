using FitRadar.Shared.DTOs;
using FitRadar.Shared.Models;

namespace FitRadar.Services.Interfaces
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken ct);
        Task<FacilityDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<IEnumerable<FacilityDto>> GetByTypeAsync(Shared.Models.Type type, CancellationToken ct);
        Task<FacilityDto> CreateAsync(FacilityInputDto dto, CancellationToken ct);
        Task UpdateAsync(Guid id, FacilityInputDto dto, CancellationToken ct);
        Task DeleteAsync(Guid facilityId, CancellationToken ct);
    }
}
