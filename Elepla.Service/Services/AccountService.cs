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

        #region Change Password
        public async Task<ResponseModel> ChangePasswordAsync(ChangePasswordDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Kiểm tra mật khẩu nhập vào có trùng với mật khẩu hiện tại của người dùng không
                if (!_passwordHasher.VerifyPassword(user.PasswordHash, model.CurrentPassword))
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Current password is incorrect."
                    };
                }

                // Kiểm tra mật khẩu mới có đúng định dạng không
                var passwordErrors = _passwordHasher.ValidatePassword(model.NewPassword);
                if (passwordErrors.Any())
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Password is not in correct format.",
                        Errors = passwordErrors.ToList()
                    };
                }

                // Mã hóa mật khẩu mới
                user.PasswordHash = _passwordHasher.HashPassword(model.NewPassword);

                _unitOfWork.AccountRepository.Update(user);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Password changed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while changing password.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Update User Phone Number Or Link Phone Number
        // Generate verification code
        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        // Send verification code to the new phone number when user wants to change phone number
        public async Task<ResponseModel> SendVerificationCodeAsync(NewPhoneNumberDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Kiểm tra xem số điện thoại mới đã được sử dụng chưa
                var existingUser = await _unitOfWork.AccountRepository.GetUserByPhoneNumberAsync(model.NewPhoneNumber);
                if (existingUser != null && existingUser.UserId != user.UserId)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Phone number is already in use."
                    };
                }

                // Tạo mã xác thực và gửi mã xác thực đến số điện thoại mới
                var code = GenerateVerificationCode();
                await _smsSender.SendSmsAsync(model.NewPhoneNumber, $"Mã xác thực của bạn là: {code}, mã sẽ hết hiệu lực sau 10 phút.");

                _cache.Set(model.NewPhoneNumber, code, TimeSpan.FromMinutes(10)); // Lưu mã xác thực vào cache với thời gian hết hạn 10 phút
                DateTime expiryTime = DateTime.UtcNow.ToLocalTime().AddMinutes(10); // Thời gian hết hạn 10 phút từ bây giờ

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Verification code sent to new phone number.",
                    Data = new
                    {
                        CodeExpiryTime = expiryTime
                    }
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while sending verification code.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Verify the new phone number with the verification code and update the phone number
        public async Task<ResponseModel> VerifyAndUpdateNewPhoneNumberAsync(ChangePhoneNumberDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Xác thực mã xác thực
                if (_cache.TryGetValue(model.NewPhoneNumber, out string? cachedCode) && cachedCode == model.VerificationCode)
                {
                    _cache.Remove(model.NewPhoneNumber); // Xóa mã xác thực khỏi cache

                    // Cập nhật số điện thoại mới cho người dùng
                    user.PhoneNumber = model.NewPhoneNumber;
                    user.PhoneNumberConfirmed = true;

                    _unitOfWork.AccountRepository.Update(user);
                    await _unitOfWork.SaveChangeAsync();

                    return new SuccessResponseModel<object>
                    {
                        Success = true,
                        Message = "Phone number updated successfully.",
                        Data = new
                        {
                            UserId = user.UserId,
                            NewPhoneNumber = user.PhoneNumber
                        }
                    };
                }

                return new ResponseModel
                {
                    Success = false,
                    Message = "Invalid verification code."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while verifying new phone number.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion

        #region Update User Email Or Link Email
        // Send verification code to the new email when user wants to change email
        public async Task<ResponseModel> SendVerificationCodeEmailAsync(NewEmailDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Kiểm tra xem email mới đã được sử dụng chưa
                var existingUser = await _unitOfWork.AccountRepository.GetUserByEmailAsync(model.NewEmail);
                if (existingUser != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Email is already in use."
                    };
                }

                // Tạo mã xác thực và gửi mã xác thực đến email mới
                var code = GenerateVerificationCode();
                await _emailSender.SendEmailAsync(model.NewEmail, "Verification Code", $"Mã xác thực của bạn là: {code}, mã sẽ hết hiệu lực sau 10 phút.");

                _cache.Set(model.NewEmail, code, TimeSpan.FromMinutes(10)); // Lưu mã xác thực vào cache với thời gian hết hạn 10 phút
                DateTime expiryTime = DateTime.UtcNow.ToLocalTime().AddMinutes(10); // Thời gian hết hạn 10 phút từ bây giờ

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Verification code sent to new email.",
                    Data = new
                    {
                        CodeExpiryTime = expiryTime
                    }
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while sending verification code.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Verify the new email with the verification code
        public async Task<ResponseModel> VerifyAndUpdateNewEmailAsync(ChangeEmailDTO model)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Xác thực mã xác thực
                if (_cache.TryGetValue(model.NewEmail, out string? cachedCode) && cachedCode == model.VerificationCode)
                {
                    _cache.Remove(model.NewEmail); // Xóa mã xác thực khỏi cache

                    // Cập nhật email mới cho người dùng
                    user.Email = model.NewEmail;
                    user.EmailConfirmed = true;

                    _unitOfWork.AccountRepository.Update(user);
                    await _unitOfWork.SaveChangeAsync();

                    return new SuccessResponseModel<object>
                    {
                        Success = true,
                        Message = "Email updated successfully.",
                        Data = new
                        {
                            UserId = user.UserId,
                            NewEmail = user.Email
                        }
                    };
                }

                return new ResponseModel
                {
                    Success = false,
                    Message = "Invalid verification code."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while verifying new email.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        #endregion
    }
}
