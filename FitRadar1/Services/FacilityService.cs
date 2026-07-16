using FitRadar.Repositories.Interfaces;
using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs;
using FitRadar.Shared.Models;

namespace FitRadar.Services
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _repo;

        public FacilityService(IFacilityRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken ct)
        {
            var facilities = await _repo.GetAllAsync(ct);
            return facilities.Select(MapToDto);
        }

        public async Task<FacilityDto?> GetByIdAsync(Guid facilityId, CancellationToken ct)
        {
            var facility = await _repo.GetByIdAsync(facilityId, ct);
            return facility == null ? null : MapToDto(facility);
        }

        public async Task<IEnumerable<FacilityDto>> GetByTypeAsync(Shared.Models.Type type, CancellationToken ct)
        {
            var facilities = await _repo.GetByTypeAsync(type, ct);
            return facilities.Select(MapToDto);
        }

        public async Task<FacilityDto> CreateAsync(FacilityInputDto dto, CancellationToken ct)
        {
            var facility = new Facility
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Type = dto.Type,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Packages = new List<Package>()
            };

            await _repo.CreateAsync(facility, ct);
            return MapToDto(facility);
        }

        public async Task UpdateAsync(Guid facilityId, FacilityInputDto dto, CancellationToken ct)
        {
            var facility = await _repo.GetByIdAsync(facilityId, ct);
            if (facility == null)
                throw new KeyNotFoundException($"Facility with id {facilityId} not found");

            facility.Name = dto.Name;
            facility.Address = dto.Address;
            facility.Type = dto.Type;
            facility.Latitude = dto.Latitude;
            facility.Longitude = dto.Longitude;

            await _repo.UpdateAsync(facility, ct);
        }

        public async Task DeleteAsync(Guid facilityId, CancellationToken ct)
        {
            await _repo.DeleteAsync(facilityId, ct);
        }

        private static FacilityDto MapToDto(Facility facility)
        {
            return new FacilityDto
            {
                Id = facility.Id,
                Name = facility.Name,
                Address = facility.Address,
                Type = facility.Type,
                Latitude = facility.Latitude,
                Longitude = facility.Longitude,
                PackageIds = facility.Packages
                    .Select(p => p.Id)
                    .ToList()
            };
        }
    }
}
