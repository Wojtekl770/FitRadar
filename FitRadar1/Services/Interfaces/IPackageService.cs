using FitRadar.Shared.DTOs;

namespace FitRadar.Services.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageDto>> GetAllAsync(CancellationToken ct);
        Task<PackageDto?> GetByIdAsync(Guid packageId, CancellationToken ct);
        Task<IEnumerable<PackageDto>> GetByProviderIdAsync(Guid providerId, CancellationToken ct);
        Task<PackageDto> CreateAsync(PackageInputDto dto, CancellationToken ct);
        Task UpdateAsync(Guid packageId, PackageInputDto dto, CancellationToken ct);
        Task DeleteAsync(Guid packageId, CancellationToken ct);
        Task AddFacilityToPackageAsync(Guid facilityId, Guid packageId, CancellationToken ct);
        Task RemoveFacilityFromPackageAsync(Guid facilityId, Guid packageId, CancellationToken ct);
        Task AddUserToPackageAsync(Guid userId, Guid packageId, CancellationToken ct);
        Task RemoveUserFromPackageAsync(Guid userId, Guid packageId, CancellationToken ct);
    }
}
