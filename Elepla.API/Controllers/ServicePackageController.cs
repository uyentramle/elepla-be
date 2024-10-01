using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elepla.API.Controllers
{
    public class ServicePackageController : BaseController
    {
        private readonly IServicePackageService _servicePackageService;

        public ServicePackageController(IServicePackageService servicePackageService)
        {
            _servicePackageService = servicePackageService;
        }

        // Get all service packages
        [HttpGet]
        public async Task<IActionResult> GetAllServicePackagesAsync()
        {
            var packages = await _servicePackageService.GetAllServicePackagesAsync();
            return Ok(packages);
        }

        // Get a service package by ID
        [HttpGet("{packageId}")]
        public async Task<IActionResult> GetServicePackageByIdAsync(string packageId)
        {
            var package = await _servicePackageService.GetServicePackageByIdAsync(packageId);
            if (package == null) return NotFound("Package not found.");
            return Ok(package);
        }

        // Add a new service package
        [HttpPost]
        public async Task<IActionResult> AddServicePackageAsync([FromBody] CreateServicePackageDTO packageDTO)
        {
            await _servicePackageService.AddServicePackageAsync(packageDTO);
            return Ok("Service package added successfully.");
        }

        // Update an existing service package
        [HttpPut("{packageId}")]
        public async Task<IActionResult> UpdateServicePackageAsync(string packageId, [FromBody] UpdateServicePackageDTO packageDTO)
        {
            // Call the service to update the package with the packageId from query
            await _servicePackageService.UpdateServicePackageAsync(packageId, packageDTO);
            return Ok("Service package updated successfully.");
        }


        // Delete a service package
        [HttpDelete("{packageId}")]
        public async Task<IActionResult> DeleteServicePackageAsync(string packageId)
        {
            await _servicePackageService.DeleteServicePackageAsync(packageId);
            return Ok("Service package deleted successfully.");
        }
    }
}
