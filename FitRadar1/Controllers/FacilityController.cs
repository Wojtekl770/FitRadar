using FitRadar.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FitRadar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _service;

        public FacilityController(IFacilityService service)
        {
            _service = service;
        }

        /// <summary>
        ///  Gets all facilities.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var facilities = await _service.GetAllAsync(ct);
            return Ok(facilities);
        }


        /// <summary>
        /// Get facility by its ID.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{facilityId:guid}")]
        public async Task<IActionResult> GetById(Guid facilityId, CancellationToken ct)
        {
            var facility = await _service.GetByIdAsync(facilityId, ct);
            if (facility == null)
            {
                return NotFound();
            }
            return Ok(facility);
        }

        /// <summary>
        /// Get facilites by their type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type, CancellationToken ct)
        {
            if (!Enum.TryParse<Shared.Models.Type>(type, true, out var parsedType))
            {
                return BadRequest("Invalid type parameter.");
            }
            var facilities = await _service.GetByTypeAsync(parsedType, ct);
            return Ok(facilities);
        }

        /// <summary>
        /// Create a new facility. Admin only.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Shared.DTOs.FacilityInputDto dto, CancellationToken ct)
        {
            var createdFacility = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { facilityId = createdFacility.Id }, createdFacility);
        }

        /// <summary>
        /// Update an existing facility. Admin only.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPut("{facilityId:guid}")]
        public async Task<IActionResult> Update(Guid facilityId, [FromBody] Shared.DTOs.FacilityInputDto dto, CancellationToken ct)
        {
            try
            {
                await _service.UpdateAsync(facilityId, dto, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Delete a facility by its ID. Admin only.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("{facilityId:guid}")]
        public async Task<IActionResult> Delete(Guid facilityId, CancellationToken ct)
        {
            await _service.DeleteAsync(facilityId, ct);
            return NoContent();
        }
    }
}