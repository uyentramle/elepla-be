using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.ServicePackageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // Get all service packages
        public async Task<IEnumerable<ServicePackage>> GetAllServicePackagesAsync()
        {
            return await _unitOfWork.ServicePackageRepository.GetAllServicePackagesAsync();
        }

        // Get a service package by its ID
        public async Task<ServicePackage?> GetServicePackageByIdAsync(string packageId)
        {
            return await _unitOfWork.ServicePackageRepository.GetServicePackageByIdAsync(packageId);
        }

        // Add a new service package using DTO
        public async Task AddServicePackageAsync(CreateServicePackageDTO packageDTO)
        {
            var package = _mapper.Map<ServicePackage>(packageDTO); // Map DTO to entity

            // Set timestamps and user info
            package.CreatedAt = _timeService.GetCurrentTime();
            package.CreatedBy = _claimsService.GetCurrentUserId().ToString();

            await _unitOfWork.ServicePackageRepository.AddServicePackageAsync(package);
            await _unitOfWork.SaveChangeAsync();
        }

        // Update an existing service package using DTO
        public async Task UpdateServicePackageAsync(string packageId, UpdateServicePackageDTO packageDTO)
        {
            var package = await _unitOfWork.ServicePackageRepository.GetServicePackageByIdAsync(packageId);
            if (package != null)
            {
                _mapper.Map(packageDTO, package); // Map updated values to the existing entity

                // Set timestamps and user info
                package.UpdatedAt = _timeService.GetCurrentTime();
                package.UpdatedBy = _claimsService.GetCurrentUserId().ToString();

                _unitOfWork.ServicePackageRepository.UpdateServicePackage(package);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        // Delete a service package by its ID
        public async Task DeleteServicePackageAsync(string packageId)
        {
            _unitOfWork.ServicePackageRepository.DeleteServicePackage(packageId);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
