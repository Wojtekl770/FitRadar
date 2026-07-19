using FitRadar.Shared.DTOs;

namespace FitRadar.Services.Interfaces
{
    public interface IProviderService
    {
        Task<IEnumerable<ProviderDto>> GetAllAsync(CancellationToken ct);
        Task<ProviderDto?> GetByIdAsync(Guid providerId, CancellationToken ct);
        Task<ProviderDto> CreateAsync(ProviderInputDto dto, CancellationToken ct);
        Task UpdateAsync(Guid providerId, ProviderInputDto dto, CancellationToken ct);
        Task DeleteAsync(Guid providerId, CancellationToken ct);

    }
}
