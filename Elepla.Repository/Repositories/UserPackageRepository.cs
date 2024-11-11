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
    public class UserPackageRepository : GenericRepository<UserPackage>, IUserPackageRepository
    {
        public UserPackageRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
        {
        }

        public async Task<ServicePackage?> GetActiveUserPackageAsync(string userId)
        {
            var userPackage = await _dbContext.UserPackages.FirstOrDefaultAsync(up => up.UserId.Equals(userId) && up.IsActive);

            if (userPackage is null)
            {
                return null;
            }

            return await _dbContext.ServicePackages.FirstOrDefaultAsync(sp => sp.PackageId.Equals(userPackage.PackageId));
        }
    }
}
