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

      
    }
}
