using FitRadar.Services.Interfaces;
using FitRadar.Shared.DTOs;
using FitRadar.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitRadar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _service;

        public PackageController(IPackageService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all packages.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var packages = await _service.GetAllAsync(ct);
            return Ok(packages);
        }

        /// <summary>
        /// Get package by its ID.
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("{packageId:guid}")]
        public async Task<IActionResult> GetById(Guid packageId, CancellationToken ct)
        {
            var package = await _service.GetByIdAsync(packageId, ct);
            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }

        /// <summary>
        /// Get packages by package ID.
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("provider/{packageId:guid}")]
        public async Task<IActionResult> GetByProviderId(Guid packageId, CancellationToken ct)
        {
            var packages = await _service.GetByProviderIdAsync(packageId, ct);
            return Ok(packages);
        }

        /// <summary>
        /// Create a new package.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(PackageInputDto dto, CancellationToken ct)
        {
            var package = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { providerId = package.Id }, package);
        }

        /// <summary>
        /// Update an existing package.
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPut("{packageId:guid}")]
        public async Task<IActionResult> Update(Guid packageId, PackageInputDto dto, CancellationToken ct)
        {
            await _service.UpdateAsync(packageId, dto, ct);
            return NoContent();
        }

        /// <summary>
        /// Delete a package by its ID.
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("{packageId:guid}")]
        public async Task<IActionResult> Delete(Guid packageId, CancellationToken ct)
        {
            await _service.DeleteAsync(packageId, ct);
            return NoContent();
        }

        /// <summary>
        /// Add a facility to a package.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("{packageId:guid}/facilities/{facilityId:guid}")]
        public async Task<IActionResult> AddFacilityToPackage(Guid facilityId, Guid packageId, CancellationToken ct)
        {
            await _service.AddFacilityToPackageAsync(facilityId, packageId, ct);
            return NoContent();
        }

        /// <summary>
        /// Remove a facility from a package.
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("{packageId:guid}/facilities/{facilityId:guid}")]
        public async Task<IActionResult> RemoveFacilityFromPackage(Guid facilityId, Guid packageId, CancellationToken ct)
        {
            await _service.RemoveFacilityFromPackageAsync(facilityId, packageId, ct);
            return NoContent();
        }

        /// <summary>
        /// Add a user to a package.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("{packageId:guid}/users/{userId:guid}")]
        public async Task<IActionResult> AddUserToPackage(Guid userId, Guid packageId, CancellationToken ct)
        {
            await _service.AddUserToPackageAsync(userId, packageId, ct);
            return NoContent();
        }

        /// <summary>
        /// Remove a user from a package.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="packageId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("{packageId:guid}/users/{userId:guid}")]
        public async Task<IActionResult> RemoveUserFromPackage(Guid userId, Guid packageId, CancellationToken ct)
        {
            await _service.RemoveUserFromPackageAsync(userId, packageId, ct);
            return NoContent();
        }
    }
}
