using Elepla.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
	public class ActivityController : BaseController
	{
		private readonly IActivityService _activityService;

		public ActivityController(IActivityService activityService)
		{
			_activityService = activityService;
		}

		[HttpGet]
		public async Task<IActionResult> GetActivitiesByPlanbookIdAsync(string planbookId)
		{
			var response = await _activityService.GetAllByPlanbookIdAsync(planbookId);
			return Ok(response);
		}
	}
}
