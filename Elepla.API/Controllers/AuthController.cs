using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.AuthViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Login
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDTO model)
        {
            if (ModelState.IsValid)
            {
                var respone = await _authService.LoginAsync(model);

                if (respone.Success)
                {
                    return Ok(respone);
                }

                return Unauthorized(respone);
            }

            var errors = ModelState.ToDictionary(
                key => key.Key,
                value => string.Join("; ", value.Value.Errors.Select(e => e.ErrorMessage)));

            return BadRequest(new ErrorResponseModel<Dictionary<string, string>>
            {
                Success = false,
                Message = "Invalid request",
                Errors = errors.Values.ToList()
            });
        }
        #endregion
    }
}
