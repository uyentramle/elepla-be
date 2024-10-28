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
        Task<ResponseModel> GetUserPackagesAsync(string userId);
        Task<ResponseModel> GetUserPackageDetailsAsync(int userPackageId);
    }
}
