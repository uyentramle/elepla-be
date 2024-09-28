using AutoMapper;
using Elepla.Domain.Entities;
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
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating user profile.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Update user avatar
        public async Task<ResponseModel> UpdateUserAvatarAsync(UpdateUserAvatarDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);

                if (user == null)
                {
                    return new ResponseModel { Success = false, Message = "User not found." };
                }

                // Nếu người dùng đã có AvatarId, cập nhật avatar cũ
                if (!string.IsNullOrEmpty(user.AvatarId))
                {
                    var oldAvatar = await _unitOfWork.ImageRepository.GetByIdAsync(user.AvatarId);
                    if (oldAvatar != null)
                    {
                        oldAvatar.ImageUrl = model.AvatarUrl;
                        _unitOfWork.ImageRepository.Update(oldAvatar);
                    }
                }
                else
                {
                    // Nếu người dùng chưa có Avatar, thêm ảnh mới
                    var newAvatar = new Image
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        ImageUrl = model.AvatarUrl,
                        Type = "Avatar",
                        CreatedBy = user.UserId
                    };

                    await _unitOfWork.ImageRepository.AddAsync(newAvatar);
                    await _unitOfWork.SaveChangeAsync();

                    // Gán AvatarId cho người dùng
                    user.AvatarId = newAvatar.ImageId;
                    _unitOfWork.AccountRepository.Update(user);
                }

                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel { Success = true, Message = "User avatar updated successfully." };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating user avatar.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> UpdateAvatarAsync(UpdateAvatarDTO model)
        {
            try
            {
                // Tìm người dùng dựa trên các tiêu chí tìm kiếm khác nhau
                var user = await _unitOfWork.AccountRepository.FindByAnyCriteriaAsync(
                    model.Email, model.PhoneNumber, model.UserName, model.GoogleEmail, model.FacebookEmail
                );

                if (user == null)
                {
                    return new ResponseModel { Success = false, Message = "User not found." };
                }

                // Tạo một bản ghi mới cho hình ảnh avatar
                var newAvatar = new Image
                {
                    ImageId = Guid.NewGuid().ToString(),
                    ImageUrl = model.AvatarUrl,
                    Type = "Avatar",
                    CreatedBy = user.UserId
                };

                await _unitOfWork.ImageRepository.AddAsync(newAvatar);
                await _unitOfWork.SaveChangeAsync();

                // Cập nhật avatar cho user
                user.AvatarId = newAvatar.ImageId;

                _unitOfWork.AccountRepository.Update(user);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel { Success = true, Message = "Avatar updated successfully." };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating avatar.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion
    }
}
