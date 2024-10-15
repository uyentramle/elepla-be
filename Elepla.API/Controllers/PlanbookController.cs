using Elepla.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
	public class PlanbookController : BaseController
	{
		private readonly IPlanbookService _planbookService;

		public PlanbookController(IPlanbookService planbookService)
		{
			_planbookService = planbookService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllPlanbooksAsync(int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookService.GetAllPlanbooksAsync(pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetPlanbookByIdAsync(string planbookId)
		{
			var response = await _planbookService.GetPlanbookByIdAsync(planbookId);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetPlanbookByCollectionIdAsync(string collectionId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookService.GetPlanbookByCollectionIdAsync(collectionId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetPlanbookByLessonIdAsync(string lessonId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookService.GetPlanbookByLessonIdAsync(lessonId, pageIndex, pageSize);
			return Ok(response);
		}
	}
}
