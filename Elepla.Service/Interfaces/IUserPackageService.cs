using Elepla.Domain.Entities;
using Elepla.Service.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Interfaces
{
    public interface IUserPackageService
    {
        Task<ResponseModel> GetAllUserPackagesAsync(string? keyword, int pageIndex, int pageSize);
        Task<ResponseModel> GetUserPackagesByUserIdAsync(string userId);
        Task<ResponseModel> GetUserPackageByIdAsync(string userPackageId);
        Task<ResponseModel> AddFreePackageToUserAsync(string userId);
        Task<ResponseModel> GetActiveUserPackageByUserIdAsync(string userId);
        Task DeactivateActiveUserPackagesAsync(string userId);
        Task ActivateUserPackageAsync(string userPackageId);
        Task<ResponseModel> DeactivateExpiredUserPackagesAsync();
    }
}
