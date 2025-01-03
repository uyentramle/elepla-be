using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using Elepla.Service.Models.ViewModels.UserPackageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class UserPackageService : IUserPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeService _timeService;

        public UserPackageService(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeService = timeService;
        }

        // Get all user packages
        public async Task<ResponseModel> GetAllUserPackagesAsync(string? keyword, int pageIndex, int pageSize)
        {
            var userPackages = await _unitOfWork.UserPackageRepository.GetAsync(
                filter: up => !up.IsDeleted && (string.IsNullOrEmpty(keyword) || up.Package.PackageName.Contains(keyword)) && up.User.Role.Name != "Admin" && up.User.Role.Name != "AcademicStaff" && up.User.Role.Name != "Manager",
                orderBy: up => up.OrderByDescending(p => p.CreatedAt),
                includeProperties: "Package,User,Payments",
                pageIndex: pageIndex,
                pageSize: pageSize);

            var userPackageDtos = _mapper.Map<Pagination<ViewListUserPackageDTO>>(userPackages);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "User packages retrieved successfully.",
                Data = userPackageDtos
            };
        }

        // Get all user packages for a specific user
        public async Task<ResponseModel> GetUserPackagesByUserIdAsync(string userId)
        {
            // Check if the user exists
            var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "User ID not found."
                };
            }

            // Retrieve user packages
            var userPackages = await _unitOfWork.UserPackageRepository.GetAllAsync(
                filter: up => up.UserId.Equals(userId) && !up.IsDeleted,
                orderBy: up => up.OrderByDescending(p => p.CreatedAt),
                includeProperties: "Package,User,Payments");

            var userPackageDtos = _mapper.Map<List<ViewListUserPackageDTO>>(userPackages);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "User packages retrieved successfully.",
                Data = userPackageDtos
            };
        }

        // Get details of a specific user package
        public async Task<ResponseModel> GetUserPackageByIdAsync(string userPackageId)
        {
            var userPackage = await _unitOfWork.UserPackageRepository.GetByIdAsync(
                                            id: userPackageId,
                                            includeProperties: "Package,User,Payments");

            if (userPackage is null || userPackage.IsDeleted)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "User package not found."
                };
            }

            var userPackageDto = _mapper.Map<ViewListUserPackageDTO>(userPackage);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "User package details retrieved successfully.",
                Data = userPackageDto
            };
        }

        // Add free package to user
        // Hàm này được gọi khi người dùng đăng ký tài khoản thành công
        public async Task<ResponseModel> AddFreePackageToUserAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
                if (user is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                var existingUserPackages = await _unitOfWork.UserPackageRepository.GetAllAsync(
                                                        filter: up => up.UserId.Equals(userId) && up.Package.PackageName.Equals("Gói miễn phí") && up.IsActive);

                if (existingUserPackages.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Free package already added to user."
                    };
                }

                var freePackage = await _unitOfWork.ServicePackageRepository.ServicePackageExistsAsync("Gói miễn phí");
                if (freePackage is null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Free package not found."
                    };
                }

                var userPackage = new UserPackage
                {
                    UserPackageId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    PackageId = freePackage.PackageId,
                    StartDate = _timeService.GetCurrentTime(),
                    EndDate = freePackage.EndDate,
                    IsActive = true
                };

                await _unitOfWork.UserPackageRepository.AddAsync(userPackage);
                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Free package added to user successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while adding free package to user.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ResponseModel> GetActiveUserPackageByUserIdAsync(string userId)
        {
            var userPackage = await _unitOfWork.UserPackageRepository.GetActiveUserPackageAsync(userId);

            if (userPackage is null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = "User package not found."
                };
            }

            var userPackageDto = _mapper.Map<ViewServicePackageDTO>(userPackage);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Current user package retrieved successfully.",
                Data = userPackageDto
            };
        }

        public async Task DeactivateActiveUserPackagesAsync(string userId)
        {
            var activeUserPackages = await _unitOfWork.UserPackageRepository.GetAllAsync(up => up.UserId.Equals(userId) && up.IsActive);

            if (activeUserPackages.Any())
            {
                foreach (var userPackage in activeUserPackages)
                {
                    userPackage.IsActive = false;
                    _unitOfWork.UserPackageRepository.Update(userPackage);
                }
            }

            await _unitOfWork.SaveChangeAsync();
        }

        public async Task ActivateUserPackageAsync(string userPackageId)
        {
            var userPackage = await _unitOfWork.UserPackageRepository.GetByIdAsync(userPackageId);
            if (userPackage is not null)
            {
                userPackage.IsActive = true;
                _unitOfWork.UserPackageRepository.Update(userPackage);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task<ResponseModel> DeactivateExpiredUserPackagesAsync()
        {
            try
            {
                var expiredUserPackages = await _unitOfWork.UserPackageRepository.GetAllAsync(up => up.EndDate <= _timeService.GetCurrentTime() && up.IsActive);

                if (expiredUserPackages.Any())
                {
                    foreach (var userPackage in expiredUserPackages)
                    {
                        userPackage.IsActive = false;
                        _unitOfWork.UserPackageRepository.Update(userPackage);
                    }
                }

                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Expired user packages deactivated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while deactivating expired user packages.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
