using Elepla.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elepla.API.Controllers
{
    public class UserPackageController : BaseController
    {
        private readonly IUserPackageService _userPackageService;

        public UserPackageController(IUserPackageService userPackageService)
        {
            _userPackageService = userPackageService;
        }

        #region View Active Service Package
        [HttpGet("{userId}/active")]
        //[Authorize]
        public async Task<IActionResult> GetActivePackageAsync(string userId)
        {
            var package = await _userPackageService.GetActivePackageAsync(userId);
            if (package == null) return NotFound("No active package found.");
            return Ok(package);
        }
        #endregion

        #region View All Service Packages
        [HttpGet("{userId}/all")]
        //[Authorize]
        public async Task<IActionResult> GetAllPackagesAsync(string userId)
        {
            var packages = await _userPackageService.GetAllPackagesAsync(userId);
            if (packages == null || !packages.Any()) return NotFound("No packages found.");
            return Ok(packages);
        }
        #endregion

        #region Purchase Service Package
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> PurchasePackageAsync(string userId, string packageId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(packageId))
            {
                return BadRequest("Invalid userId or packageId.");
            }

            var result = await _userPackageService.PurchaseServicePackageAsync(userId, packageId);
            if (result)
            {
                return Ok("Package purchased successfully.");
            }
            return BadRequest("Purchase failed.");
        }
        #endregion

        #region Renew Service Package
        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> RenewPackageAsync(string userId, string packageId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(packageId))
            {
                return BadRequest("Invalid userId or packageId.");
            }

            var result = await _userPackageService.RenewServicePackageAsync(userId, packageId);
            if (result)
            {
                return Ok("Package renewed successfully.");
            }
            return BadRequest("Renewal failed.");
        }
        #endregion

        #region Upgrade Service Package
        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> UpgradePackageAsync(string userId, string newPackageId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPackageId))
            {
                return BadRequest("Invalid userId or newPackageId.");
            }

            var result = await _userPackageService.UpgradeServicePackageAsync(userId, newPackageId);
            if (result)
            {
                return Ok("Package upgraded successfully.");
            }
            return BadRequest("Upgrade failed.");
        }
        #endregion
    }
}
