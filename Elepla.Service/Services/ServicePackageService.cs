﻿using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Common;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ResponseModels;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class ServicePackageService : IServicePackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServicePackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Get all service packages with pagination and optional filtering by name
        public async Task<ResponseModel> GetAllServicePackagesAsync(string? keyword, int pageIndex, int pageSize)
        {
            var packages = await _unitOfWork.ServicePackageRepository.GetAsync(
                                filter: p => !p.IsDeleted && (string.IsNullOrEmpty(keyword) || p.PackageName.Contains(keyword)),
                                orderBy: p => p.OrderBy(p => p.Price),
                                pageIndex: pageIndex,
                                pageSize: pageSize
            );

            var packageDtos = _mapper.Map<Pagination<ViewServicePackageDTO>>(packages);

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
                return new ResponseModel
                {
                    Success = false,
                    Message = "Service package not found."
                };
            }

            var result = _mapper.Map<ViewServicePackageDTO>(package);

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

                await _unitOfWork.ServicePackageRepository.AddAsync(package);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Service package created successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the service package.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        // Update an existing service package
        public async Task<ResponseModel> UpdateServicePackageAsync(UpdateServicePackageDTO model)
        {
            try
            {
                var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(model.PackageId);
                if (package == null)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Service package not found."
                    };
                }

                if (package.IsDeleted == true)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Cannot modify a deleted service package."
                    };
                }

                _mapper.Map(model, package);

                _unitOfWork.ServicePackageRepository.Update(package);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Service package updated successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the service package.",
                    Errors = new List<string> { ex.Message }
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
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Service package not found."
                    };
                }

                if (package.IsDeleted == true)
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = "Service package is already deleted."
                    };
                }

                _unitOfWork.ServicePackageRepository.SoftRemove(package);
                await _unitOfWork.SaveChangeAsync();

                return new ResponseModel
                {
                    Success = true,
                    Message = "Service package deleted successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponseModel<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the service package.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
