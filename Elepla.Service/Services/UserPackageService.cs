using AutoMapper;
using Elepla.Domain.Entities;
using Elepla.Repository.Interfaces;
using Elepla.Service.Interfaces;
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

        public UserPackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Method to purchase a new service package for the user
        public async Task<bool> PurchaseServicePackageAsync(string userId, string packageId)
        {
            var user = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(packageId);
            if (package == null) return false;

            var newUserPackage = new UserPackage
            {
                UserId = user.UserId,
                PackageId = package.PackageId,
                StartDate = System.DateTime.UtcNow,
                EndDate = System.DateTime.UtcNow.AddMonths(package.Duration),
                IsActive = true
            };

            await _unitOfWork.UserPackageRepository.AddAsync(newUserPackage);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        // Method to renew an existing package
        public async Task<bool> RenewServicePackageAsync(string userId, string packageId)
        {
            var userPackage = await _unitOfWork.UserPackageRepository.GetActiveUserPackageAsync(userId);
            if (userPackage == null) return false;

            var package = await _unitOfWork.ServicePackageRepository.GetByIdAsync(packageId);
            if (package == null) return false;

            userPackage.EndDate = userPackage.EndDate.AddMonths(package.Duration);

            _unitOfWork.UserPackageRepository.Update(userPackage);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        // Method to upgrade a package
        public async Task<bool> UpgradeServicePackageAsync(string userId, string newPackageId)
        {
            var userPackage = await _unitOfWork.UserPackageRepository.GetActiveUserPackageAsync(userId);
            if (userPackage != null)
            {
                userPackage.IsActive = false;
                _unitOfWork.UserPackageRepository.Update(userPackage);
            }

            return await PurchaseServicePackageAsync(userId, newPackageId);
        }

    }
}
