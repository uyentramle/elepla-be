using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class ServicePackageService : IServicePackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeService _timeService;
        private readonly IClaimsService _claimsService;

        public ServicePackageService(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeService = timeService;
            _claimsService = claimsService;
        }

        // Get all service packages with pagination
        public async Task<ResponseModel> GetAllServicePackagesAsync(int pageIndex, int pageSize)
        {
            var packages = await _unitOfWork.ServicePackageRepository.GetAsync(
                filter: r => !r.IsDeleted, // Filtering out deleted packages
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var packageDtos = _mapper.Map<Pagination<ServicePackageDTO>>(packages);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Service packages retrieved successfully.",
                Data = packageDtos
            };
        }

        // Get a service package by its ID
        public async Task<ResponseModel> GetServicePackageByIdAsync(string packageId)
        {
            var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(packageId);

            if (package == null)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "Service package not found."
                };
            }

            var result = _mapper.Map<ServicePackageDTO>(package);

            return new SuccessResponseModel<object>
            {
                Success = true,
                Message = "Service package retrieved successfully.",
                Data = result
            };
        }

        // Add a new service package
        public async Task<ResponseModel> AddServicePackageAsync(CreateServicePackageDTO model)
        {
            try
            {
                var package = _mapper.Map<ServicePackage>(model);
                package.PackageId = Guid.NewGuid().ToString(); // Generating new ID
                package.CreatedAt = _timeService.GetCurrentTime();
                package.CreatedBy = _claimsService.GetCurrentUserId().ToString();

                await _unitOfWork.ServicePackageRepository.AddAsync(package);
                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Service package created successfully.",
                    Data = package
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // Update an existing service package
        public async Task<ResponseModel> UpdateServicePackageAsync(string packageId, UpdateServicePackageDTO model)
        {
            try
            {
                var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Service package not found."
                    };
                }

                if (package.IsDeleted == true)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Cannot modify a deleted service package."
                    };
                }

                _mapper.Map(model, package);
                package.UpdatedAt = _timeService.GetCurrentTime();
                package.UpdatedBy = _claimsService.GetCurrentUserId().ToString();

                _unitOfWork.ServicePackageRepository.Update(package);
                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Service package updated successfully.",
                    Data = package
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // Soft delete a service package
        public async Task<ResponseModel> DeleteServicePackageAsync(string packageId)
        {
            try
            {
                var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Service package not found."
                    };
                }

                if (package.IsDeleted == true)
                {
                    return new ErrorResponseModel<object>
                    {
                        Success = false,
                        Message = "Service package is already deleted."
                    };
                }

                _unitOfWork.ServicePackageRepository.SoftRemove(package);
                await _unitOfWork.SaveChangeAsync();

                return new SuccessResponseModel<object>
                {
                    Success = true,
                    Message = "Service package deleted successfully.",
                    Data = package
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
