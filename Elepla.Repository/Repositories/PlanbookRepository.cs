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
    public class PlanbookRepository : GenericRepository<Planbook>, IPlanbookRepository
    {
        public PlanbookRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
        {         
        }

        //public async Task<int> CountPlanbookByUserId(string userId)
        //{
        //    return await _dbContext.Planbooks.Include(p => p.PlanbookCollection)
        //                                    .Where(p => p.PlanbookCollection.TeacherId.Equals(userId))
        //                                    .CountAsync();
        //}

        public async Task<int> CountPlanbookByUserId(string userId)
        {
            return await _dbContext.PlanbookInCollections
                                    .Include(pic => pic.Planbook)
                                    .Include(pic => pic.PlanbookCollection)
                                    .Where(pic => pic.PlanbookCollection.TeacherId.Equals(userId))
                                    .CountAsync();
        }
    }
}
