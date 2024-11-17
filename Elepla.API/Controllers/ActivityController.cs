using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.ActivityViewModels;
using Microsoft.AspNetCore.Authorization;
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

		[HttpGet]
		public async Task<IActionResult> GetActivityByIdAsync(string activityId)
		{
			var response = await _activityService.GetActivityByIdAsync(activityId);
			return Ok(response);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateActivityAsync(CreateActivityDTO model)
		{
			var response = await _activityService.CreateActivityAsync(model);
			return Ok(response);
		}

		[HttpPut]
		[Authorize]
		public async Task<IActionResult> UpdateActivityAsync(UpdateActivityDTO model)
		{
			var response = await _activityService.UpdateActivityAsync(model);
			return Ok(response);
		}

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> DeleteActivityAsync(string activityId)
		{
			var response = await _activityService.DeleteActivityAsync(activityId);
			return Ok(response);
		}
	}
}
