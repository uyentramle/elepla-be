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
	public class PlanbookCollectionRepository : GenericRepository<PlanbookCollection>, IPlanbookCollectionRepository
	{
		public PlanbookCollectionRepository(AppDbContext dbContext, ITimeService timeService, IClaimsService claimsService) : base(dbContext, timeService, claimsService)
		{
		}

		public async Task<bool> CheckPlanbookCollectionIsSavedExistByTeacherId(string teacherId)
		{
			return await _dbContext.PlanbookCollections.AnyAsync(c => c.TeacherId == teacherId && c.IsSaved);
		}

		public async Task<bool> CheckCollectionByName(string collectionName, string teacherId)
		{
            return await _dbContext.PlanbookCollections.AnyAsync(c => c.CollectionName.Equals(collectionName) && c.TeacherId.Equals(teacherId));
        }
	}
}
