using Elepla.Domain.Entities;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class ServicePackageRepository : GenericRepository<ServicePackage>, IServicePackageRepository
    {
        public ServicePackageRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService)
            : base(dbContext, timeService, claimsService)
        {
        }

        public async Task<ServicePackage?> ServicePackageExistsAsync(string servicePackageName)
        {
            return await _dbContext.ServicePackages.FirstOrDefaultAsync(r => r.PackageName.Equals(servicePackageName));
        }
    }
}
