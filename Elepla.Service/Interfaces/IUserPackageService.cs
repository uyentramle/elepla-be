using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IUserPackageService
    {
        Task<bool> PurchaseServicePackageAsync(string userId, string packageId);
        Task<bool> RenewServicePackageAsync(string userId, string packageId);
        Task<bool> UpgradeServicePackageAsync(string userId, string newPackageId);
    }
}
