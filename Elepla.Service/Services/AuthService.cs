using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.AuthViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly ITimeService _currentTime;
        private readonly IPasswordService _passwordHasher;

        public AuthService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            AppConfiguration appConfiguration,
            ITimeService currentTime,
            IPasswordService passwordHasher) 
            : base(unitOfWork, mapper, appConfiguration)
        {
            _currentTime = currentTime;
            _passwordHasher = passwordHasher;
        }

        #region Login
        public async Task<ResponseModel> LoginAsync(LoginDTO model)
        {
            var user = await _unitOfWork.AccountRepository.GetUserByEmailOrUserNameOrPhoneNumberAsync(model.Username);

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
                    user.LastLogin = _currentTime.GetCurrentTime();

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

            tokenExpiryTime = _currentTime.GetCurrentTime().AddMinutes(jwt.AccessTokenDurationInMinutes);

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

    }
}
