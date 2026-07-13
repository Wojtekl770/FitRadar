using FitRadar.Repositories.Interfaces;
using FitRadar.Shared.DTOs;
using FitRadar.Shared.Models;

namespace FitRadar.Services.Interfaces
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
            var facility = await _repo.GetAllAsync(ct);
            return facility.Select(MapToDto);
        }


        public async Task<FacilityDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var facility = await _repo.GetByIdAsync(id, ct);
            return facility == null ? null : MapToDto(facility);
        }

        public async Task<IEnumerable<FacilityDto?>> GetByTypeAsync(Shared.Models.Type type, CancellationToken ct)
        {
            var facility = await _repo.GetByTypeAsync(type, ct);
            return facility == null ? null : facility.Select(MapToDto); // tutaj do poprawki
        }

        public async Task<FacilityDto> CreateAsync(FacilityInputDto dto, CancellationToken ct)
        {
            Facility facility = new Facility
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Adress = dto.Adress,
                Type = dto.Type,
                latitude = dto.latitude,
                longitude = dto.longitude,
                Packages = new List<Package>()
            };

            await _repo.CreateAsync(facility, ct);

            return MapToDto(facility);
        }

        public async Task UpdateAsync(Guid id, FacilityInputDto dto, CancellationToken ct)
        {
            var facility = await _repo.GetByIdAsync(id, ct);
            if (facility == null)
                throw new KeyNotFoundException("Course not found");

            facility.Name = dto.Name;
            facility.Adress = dto.Adress;
            facility.Type = dto.Type;
            facility.latitude = dto.latitude;
            facility.longitude = dto.longitude;

            await _repo.UpdateAsync(facility, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            await _repo.DeleteAsync(id, ct);
        }


        public static FacilityDto MapToDto(Facility facility)
        {
            return new FacilityDto
            {
                Id = facility.Id,
                Name = facility.Name,
                Adress = facility.Adress,
                Type = facility.Type,
                latitude = facility.latitude,
                longitude = facility.longitude,
                Packages = []
            };
        }
    }
}
