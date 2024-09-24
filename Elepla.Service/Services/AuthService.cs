using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Domain.Enums;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.AuthViewModels;
using Elepla.Service.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ITimeService timeService,
            IPasswordService passwordHasher,
            ITokenService tokenService,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IMemoryCache cache,
            AppConfiguration appConfiguration) 
            : base(unitOfWork, mapper, timeService, passwordHasher, tokenService, emailSender, smsSender, cache, appConfiguration)
        {
        }

        #region Login
        // Login
        public async Task<ResponseModel> LoginAsync(LoginDTO model)
        {
            var user = await _unitOfWork.AccountRepository.GetUserByEmailOrUsernameOrPhoneNumberAsync(model.Username);

            if (user != null)
            {
                // Kiểm tra nếu người dùng bị khóa
                if (!user.Status)
                {
                    return new AuthenticationResponseModel
                    {
                        Success = false,
                        Message = "User account is blocked. Please contact support."
                    };
                }

                // Kiểm tra mật khẩu
                var result = _passwordHasher.VerifyPassword(user.PasswordHash, model.Password);

                if (result)
                {
                    var accessToken = GenerateJsonWebToken(user, out DateTime tokenExpiryTime);
                    var refreshToken = GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_appConfiguration.JWT.RefreshTokenDurationInDays);
                    user.LastLogin = _timeService.GetCurrentTime();

                    _unitOfWork.AccountRepository.Update(user);

                    return new AuthenticationResponseModel
                    {
                        Success = true,
                        Message = "Login success",
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenExpiryTime = tokenExpiryTime
                    };
                }
                else
                {
                    return new AuthenticationResponseModel
                    {
                        Success = false,
                        Message = "Wrong password!",
                    };
                }
            }
            else
            {
                return new AuthenticationResponseModel
                {
                    Success = false,
                    Message = "User not found!",
                };
            }
        }

        // Generate JWT
        private string GenerateJsonWebToken(User user, out DateTime tokenExpiryTime)
        {
            var jwt = _appConfiguration.JWT;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.JWTSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim ("userId", user.UserId.ToString())
            };

            var roles = _unitOfWork.RoleRepository.GetByIdAsync(user.RoleId).Result.Name;

            // Thêm các claim về vai trò
            //foreach (var role in roles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //}

            claims.Add(new Claim(ClaimTypes.Role, roles));

            tokenExpiryTime = _timeService.GetCurrentTime().AddMinutes(jwt.AccessTokenDurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                expires: tokenExpiryTime,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        #endregion

        #region Register
        // Generate verification code
        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        // Send verification code to phone number or email when user register
        public async Task<ResponseModel> SendRegisterVerificationCodeAsync(PhoneNumberOrEmailDTO model)
        {
            if (IsEmail(model.PhoneNumberOrEmail))
            {
                var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(model.PhoneNumberOrEmail);
                if (user != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Email already in use."
                    };
                }

                var emailCode = GenerateVerificationCode();
                await _emailSender.SendEmailAsync(model.PhoneNumberOrEmail, "Xác thực tài khoản", $"Mã xác thực của bạn là: {emailCode}, mã sẽ hết hiệu lực sau 10 phút.");
                _cache.Set(model.PhoneNumberOrEmail, emailCode, TimeSpan.FromMinutes(10));
            }
            else if (IsPhoneNumber(model.PhoneNumberOrEmail))
            {
                var user = await _unitOfWork.AccountRepository.GetUserByPhoneNumberAsync(model.PhoneNumberOrEmail);
                if (user != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Phone number already in use."
                    };
                }

                var phoneCode = GenerateVerificationCode();
                await _smsSender.SendSmsAsync(model.PhoneNumberOrEmail, $"Mã xác thực của bạn là: {phoneCode}, mã sẽ hết hiệu lực sau 10 phút.");
                _cache.Set(model.PhoneNumberOrEmail, phoneCode, TimeSpan.FromMinutes(10));
            }
            else
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Invalid phone number or email format."
                };
            }

            DateTime expiryTime = DateTime.UtcNow.ToLocalTime().AddMinutes(10); // Thời gian hết hạn 10 phút từ bây giờ

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Verification code sent.",
                Data = new
                {
                    CodeExpiryTime = expiryTime
                }
            };
        }

        // Verify phone number or email by verification code when user register
        public async Task<ResponseModel> VerifyRegisterCodeAsync(VerifyRegisterCodeDTO model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumberOrEmail) || string.IsNullOrEmpty(model.VerificationCode))
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Phone number or email and verification code are required."
                };
            }

            if (IsEmail(model.PhoneNumberOrEmail))
            {
                // Kiểm tra email tồn tại
                var user = await _unitOfWork.AccountRepository.GetUserByEmailAsync(model.PhoneNumberOrEmail);
                if (user != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Email already in use."
                    };
                }

                // Xác thực mã
                if (_cache.TryGetValue(model.PhoneNumberOrEmail, out string savedEmailCode) && savedEmailCode == model.VerificationCode)
                {
                    _cache.Remove(model.PhoneNumberOrEmail);
                    var emailToken = _tokenService.GenerateToken(model.PhoneNumberOrEmail, "email", "register", out DateTime expiryTime);
                    return new SuccessResponseModel<object>
                    {
                        Success = true,
                        Message = "Email verified.",
                        Data = new
                        {
                            Token = emailToken,
                            TokenExpiryTime = expiryTime
                        }
                    };
                }
            }
            else if (IsPhoneNumber(model.PhoneNumberOrEmail))
            {
                // Kiểm tra số điện thoại tồn tại
                var user = await _unitOfWork.AccountRepository.GetUserByPhoneNumberAsync(model.PhoneNumberOrEmail);
                if (user != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Phone number already in use."
                    };
                }

                // Xác thực mã
                if (_cache.TryGetValue(model.PhoneNumberOrEmail, out string savedPhoneCode) && savedPhoneCode == model.VerificationCode)
                {
                    _cache.Remove(model.PhoneNumberOrEmail);
                    var phoneToken = _tokenService.GenerateToken(model.PhoneNumberOrEmail, "phone", "register", out DateTime expiryTime);
                    return new SuccessResponseModel<object>
                    {
                        Success = true,
                        Message = "Phone number verified.",
                        Data = new
                        {
                            Token = phoneToken,
                            TokenExpiryTime = expiryTime
                        }
                    };
                }
            }

            return new ResponseModel
            {
                Success = false,
                Message = "Invalid verification code."
            };
        }

        // Register
        public async Task<ResponseModel> RegisterAsync(RegisterDTO model)
        {
            if (IsEmail(model.PhoneNumberOrEmail))
            {
                if (!_tokenService.ValidateToken(model.PhoneNumberOrEmail, "email", "register", model.RegisterToken))
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Invalid or expired email registration token."
                    };
                }

                var emailExists = await _unitOfWork.AccountRepository.GetUserByEmailAsync(model.PhoneNumberOrEmail);
                if (emailExists != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = $"{model.PhoneNumberOrEmail} already exists"
                    };
                }
            }
            else if (IsPhoneNumber(model.PhoneNumberOrEmail))
            {
                if (!_tokenService.ValidateToken(model.PhoneNumberOrEmail, "phone", "register", model.RegisterToken))
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Invalid or expired phone registration token."
                    };
                }

                var phoneExists = await _unitOfWork.AccountRepository.GetUserByPhoneNumberAsync(model.PhoneNumberOrEmail);
                if (phoneExists != null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = $"{model.PhoneNumberOrEmail} already exists"
                    };
                }
            }
            else
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Invalid phone number or email format."
                };
            }

            // Kiểm tra xem username có đủ độ dài yêu cầu không (tối thiểu 6 ký tự)
            if (model.Username.Length < 6 || model.Username.Length > 20)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "Username must be between 6 and 20 characters long."
                };
            }

            // Kiểm tra username đã tồn tại chưa
            var userExists = await _unitOfWork.AccountRepository.GetUserByUsernameAsync(model.Username);
            if (userExists != null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = $"{model.Username} already exists"
                };
            }

            var user = _mapper.Map<User>(model);

            // Kiểm tra mật khẩu có đúng định dạng không
            var passwordErrors = _passwordHasher.ValidatePassword(model.Password);
            if (passwordErrors.Any())
            {
                return new ErrorResponseModel<string>
                {
                    Success = false,
                    Message = "Password is not in correct format.",
                    Errors = passwordErrors.ToList()
                };
            }

            // Băm mật khẩu
            user.PasswordHash = _passwordHasher.HashPassword(model.Password);

            // Gán email hoặc số điện thoại cho người dùng dựa vào input
            if (IsEmail(model.PhoneNumberOrEmail))
            {
                user.Email = model.PhoneNumberOrEmail;
                user.EmailConfirmed = true;
            }
            else if (IsPhoneNumber(model.PhoneNumberOrEmail))
            {
                user.PhoneNumber = model.PhoneNumberOrEmail;
                user.PhoneNumberConfirmed = true;
            }

            user.RoleId = (await _unitOfWork.RoleRepository.GetRoleByNameAsync(RoleEnums.Teacher.ToString())).RoleId;
            user.CreatedBy = user.UserId;

            await _unitOfWork.AccountRepository.AddAsync(user);
            await _unitOfWork.SaveChangeAsync();

            // Xóa token trong cache sau khi đăng ký thành công
            _cache.Remove($"{model.PhoneNumberOrEmail}_phonge_register_token");
            _cache.Remove($"{model.PhoneNumberOrEmail}_email_register_token");

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Registration success.",
                Data = new
                {
                    UserId = user.UserId,
                    FullName = user.FirstName + " " + user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.Username,
                    Email = user.Email
                }
            };
        }

        // Check email format
        private bool IsEmail(string input)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        // Check phone number format
        private bool IsPhoneNumber(string input)
        {
            var phoneNumberPattern = @"^(\+84\s?\d{9}|0\d{9})$";

            return Regex.IsMatch(input, phoneNumberPattern);
        }
        #endregion
    }
}
