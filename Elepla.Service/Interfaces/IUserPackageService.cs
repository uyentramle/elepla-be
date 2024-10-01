using Elepla.Domain.Entities;
using Elepla.Service.Models.ViewModels.UserPackageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IUserPackageService
    {
        Task<UserPackageDTO?> GetActivePackageAsync(string userId);
        Task<IEnumerable<UserPackage>> GetAllPackagesAsync(string userId);
        Task<bool> PurchaseServicePackageAsync(string userId, string packageId);
        Task<bool> RenewServicePackageAsync(string userId, string packageId);
        Task<bool> UpgradeServicePackageAsync(string userId, string newPackageId);
    }
}
