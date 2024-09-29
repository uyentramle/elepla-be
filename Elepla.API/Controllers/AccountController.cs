using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        #region View User Profile
        [HttpGet]
        public async Task<IActionResult> GetUserProfileAsync(string userId)
        {
            var response = await _accountService.GetUserProfileAsync(userId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Update User Profile
        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> UpdateUserProfileAsync(UpdateUserProfileDTO model)
        {
            if (!ModelState.IsValid)
            {
                // Xử lý lỗi nếu dữ liệu không hợp lệ
                return BadRequest(ModelState);
            }

            var response = await _accountService.UpdateUserProfileAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> UpdateUserAvatarAsync(UpdateUserAvatarDTO model)
        {
            var response = await _accountService.UpdateUserAvatarAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Change Password
        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.ChangePasswordAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion

        #region Update User Phone Number Or Link Phone Number
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> SendVerificationCodeAsync(NewPhoneNumberDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.SendVerificationCodeAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        //[Authorize]
        public async Task<IActionResult> VerifyAndUpdateNewPhoneNumberAsync(ChangePhoneNumberDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.VerifyAndUpdateNewPhoneNumberAsync(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        #endregion
    }
}
