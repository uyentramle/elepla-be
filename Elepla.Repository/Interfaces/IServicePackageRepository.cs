using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IServicePackageRepository : IGenericRepository<ServicePackage>
    {
        Task<IEnumerable<ServicePackage>> GetAllServicePackagesAsync();
        Task<ServicePackage?> GetServicePackageByIdAsync(string packageId);
        Task AddServicePackageAsync(ServicePackage package);
        void UpdateServicePackage(ServicePackage package);
        void DeleteServicePackage(string packageId);
    }
}
