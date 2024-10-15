using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
	public interface IActivityRepository
	{
		Task<List<Activity>> GetByPlanbookIdAsync(string planbookId);
		Task<Activity?> GetByIdAsync(string activityId);
		Task<Activity> CreateActivityAsync(Activity activity);
		Task<Activity> UpdateActivityAsync(Activity activity);
		Task<bool> DeleteActivityAsync(Activity activity);
	}
}
