using FitRadar.Services.Interfaces;
using FitRadar.Repositories.Interfaces;
using FitRadar.Shared.DTOs;
using System.Net.NetworkInformation;

namespace FitRadar.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IFacilityService _facilityService;
        public PackageService(IPackageRepository packageRepository, IFacilityService facilityService)
        {
            _packageRepository = packageRepository;
            _facilityService = facilityService;
        }

        public async Task<IEnumerable<PackageDto>> GetAllAsync(CancellationToken ct)
        {
            var packages = await _packageRepository.GetAllAsync(ct);
            return packages.Select(MapToDto);
        }

        public async Task<PackageDto?> GetByIdAsync(Guid packageId, CancellationToken ct)
        {
            var package = await _packageRepository.GetByIdAsync(packageId, ct);
            return package == null ? null : MapToDto(package);
        }

        public async Task<IEnumerable<PackageDto>> GetByProviderIdAsync(Guid providerId, CancellationToken ct)
        {
            var packages = await _packageRepository.GetByProviderAsync(providerId, ct);
            return packages.Select(MapToDto);
        }

        public async Task<PackageDto> CreateAsync(PackageInputDto dto, CancellationToken ct)
        {
            var package = new Shared.Models.Package
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ProviderId = dto.ProviderId,
                Facilities = new List<Shared.Models.Facility>()
            };
            await _packageRepository.CreateAsync(package, ct);
            return MapToDto(package);
        }

        public static PackageDto MapToDto(Shared.Models.Package package)
        {
            return new PackageDto
            {
                Id = package.Id,
                Name = package.Name,
                ProviderId = package.ProviderId,
                FacilityIds = package.Facilities
                    .Select(f => f.Id)
                    .ToList(),
                MonthlyPrice = package.MonthlyPrice,
                OnlyForStudents = package.OnlyForStudents,
                EntriesPerMonth = package.EntriesPerMonth
            };
        }
    }
