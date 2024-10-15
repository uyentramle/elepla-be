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
	public class ActivityRepository : IActivityRepository
	{
		private readonly AppDbContext _dbContext;

		public ActivityRepository(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Activity>> GetByPlanbookIdAsync(string planbookId)
		{
			return await _dbContext.Activities.Where(a => a.PlanbookId == planbookId).OrderBy(a => a.Index).ToListAsync();
		}

		public async Task<Activity?> GetByIdAsync(string activityId)
		{
			return await _dbContext.Activities.FirstOrDefaultAsync(a => a.ActivityId == activityId);
		}

		public async Task<Activity> CreateActivityAsync(Activity activity)
		{
			await _dbContext.Activities.AddAsync(activity);
			await _dbContext.SaveChangesAsync();
			return activity;
		}

		public async Task<Activity> UpdateActivityAsync(Activity activity)
		{
			_dbContext.Activities.Update(activity);
			await _dbContext.SaveChangesAsync();
			return activity;
		}

		public async Task<bool> DeleteActivityAsync(Activity activity)
		{
			_dbContext.Activities.Remove(activity);
			await _dbContext.SaveChangesAsync();

			return true;
		}
	}
}
