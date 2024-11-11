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

        [HttpGet]
        public async Task<IActionResult> GetAllUserPackagesAsync(string? keyword, int pageIndex, int pageSize)
        {
            var response = await _userPackageService.GetAllUserPackagesAsync(keyword, pageIndex, pageSize);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetUserPackagesAsync(string userId)
        {
            var response = await _userPackageService.GetUserPackagesAsync(userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetUserPackageDetailsAsync(string userPackageId)
        {
            var response = await _userPackageService.GetUserPackageDetailsAsync(userPackageId);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
