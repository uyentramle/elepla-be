using AutoMapper;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.AccountViewModels;
using Elepla.Service.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IPasswordService passwordHasher,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IMemoryCache cache)
            : base(unitOfWork, mapper, timeService, passwordHasher, emailSender, smsSender, cache)
        {
        }

        #region Manage User Profile
        // Get user profile
        public async Task<ResponseModel> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(id: userId, includeProperties: "Role,Avatar,Background");
                if (user == null)
                {
                    return new ResponseModel { Success = false, Message = "User not found." };
                }

                var userProfileDTO = _mapper.Map<ViewUserProfileDTO>(user);
                userProfileDTO.Role = user.Role.Name;
                userProfileDTO.Avatar = user.Avatar?.ImageUrl;
                userProfileDTO.Background = user.Background?.ImageUrl;

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "User profile found.",
                    Data = userProfileDTO
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user profile.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Update user profile
        public async Task<ResponseModel> UpdateUserProfileAsync(UpdateUserProfileDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return new ResponseModel { Success = false, Message = "User not found." };
                }

                // Sử dụng mapper để cập nhật thuộc tính của đối tượng user từ model
                _mapper.Map(model, user);

                _unitOfWork.AccountRepository.Update(user);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel { Success = true, Message = "User profile updated successfully." };

            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "An error occurred while updating user profile.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        #endregion
    }
}
