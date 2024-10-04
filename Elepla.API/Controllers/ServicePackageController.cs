using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using Microsoft.AspNetCore.Authorization;
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

        #region Get All Service Packages
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllServicePackagesAsync(int pageIndex = 0, int pageSize = 10, string packageName = null)
        {
            var response = await _servicePackageService.GetAllServicePackagesAsync(pageIndex, pageSize, packageName);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Get Service Package by ID
        [HttpGet("{packageId}")]
        //[Authorize]
        public async Task<IActionResult> GetServicePackageByIdAsync(string packageId)
        {
            var response = await _servicePackageService.GetServicePackageByIdAsync(packageId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        #endregion

        #region Add New Service Package
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> AddServicePackageAsync([FromBody] CreateServicePackageDTO packageDTO)
        {
            var response = await _servicePackageService.AddServicePackageAsync(packageDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Update Service Package
        [HttpPut("{packageId}")]
        //[Authorize]
        public async Task<IActionResult> UpdateServicePackageAsync(string packageId, [FromBody] UpdateServicePackageDTO packageDTO)
        {
            var response = await _servicePackageService.UpdateServicePackageAsync(packageId, packageDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Delete Service Package
        [HttpDelete("{packageId}")]
        //[Authorize]
        public async Task<IActionResult> DeleteServicePackageAsync(string packageId)
        {
            var response = await _servicePackageService.DeleteServicePackageAsync(packageId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion
    }
}
