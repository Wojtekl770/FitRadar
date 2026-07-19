using FitRadar.Repositories.Interfaces;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs;
using FitRadar.Shared.Models;

namespace FitRadar.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;

        public ProviderService(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public async Task<IEnumerable<ProviderDto>> GetAllAsync(CancellationToken ct)
        {
            var providers = await _providerRepository.GetAllAsync(ct);
            return providers.Select(MapToDto);
        }

        public async Task<ProviderDto?> GetByIdAsync(Guid providerId, CancellationToken ct)
        {
            var provider = await _providerRepository.GetByIdAsync(providerId, ct);
            return provider == null ? null : MapToDto(provider);
        }

        public async Task<ProviderDto> CreateAsync(ProviderInputDto dto, CancellationToken ct)
        {
            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Packages = new List<Package>()
            };
            await _providerRepository.CreateAsync(provider, ct);
            return MapToDto(provider);
        }

        public async Task UpdateAsync(Guid providerId, ProviderInputDto dto, CancellationToken ct)
        {
            var provider = await _providerRepository.GetByIdAsync(providerId, ct);
            if (provider == null)
                throw new KeyNotFoundException($"Provider with id {providerId} not found");
            provider.Name = dto.Name;
            await _providerRepository.UpdateAsync(provider, ct);
        }

        public async Task DeleteAsync(Guid providerId, CancellationToken ct)
        {
            var provider = await _providerRepository.GetByIdAsync(providerId, ct);
            if (provider == null)
                throw new KeyNotFoundException($"Provider with id {providerId} not found");
            await _providerRepository.DeleteAsync(provider, ct);
        }

        private ProviderDto MapToDto(Provider provider)
        {
            return new ProviderDto
            {
                Id = provider.Id,
                Name = provider.Name,
                PackageIds = provider.Packages.Select(p => p.Id).ToList()
            };
        }

    }
}
