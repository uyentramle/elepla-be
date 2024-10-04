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

        public async Task<UserPackage?> GetActiveUserPackageAsync(string userId)
        {
            return await _dbContext.UserPackages
                .Include(up => up.Package)
                .FirstOrDefaultAsync(up => up.UserId == userId && up.IsActive);
        }
    }
}
