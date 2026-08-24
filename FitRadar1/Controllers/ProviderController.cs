using Microsoft.AspNetCore.Mvc;
using FitRadar.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FitRadar.Shared.DTOs;

namespace FitRadar.Controllers
{
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _service;

        public ProviderController(IProviderService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all providers
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var providers = await _service.GetAllAsync(ct);
            return Ok(providers);
        }

        /// <summary>
        /// Get provider by its ID.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{providerId:guid}")]
        public async Task<IActionResult> GetById(Guid providerId, CancellationToken ct)
        {
            var provider = await _service.GetByIdAsync(providerId, ct);
            if (provider == null)
            {
                return NotFound();
            }
            return Ok(provider);
        }


        /// <summary>
        /// Create a new provider.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(ProviderInputDto dto, CancellationToken ct)
        {
            var provider = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { providerId = provider.Id }, provider);
        }

        /// <summary>
        /// Update an existing provider.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPut("{providerId:guid}")]
        public async Task<IActionResult> Update(Guid providerId, ProviderInputDto dto, CancellationToken ct)
        {
            await _service.UpdateAsync(providerId, dto, ct);
            return NoContent();
        }

        /// <summary>
        /// Delete a provider by its ID.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("{providerId:guid}")]
        public async Task<IActionResult> Delete(Guid providerId, CancellationToken ct)
        {
            await _service.DeleteAsync(providerId, ct);
            return NoContent();
        }
    }
}
