using Elepla.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
	public class PlanbookCollectionController : BaseController
	{
		private readonly IPlanbookCollectionService _planbookCollectionService;

		public PlanbookCollectionController(IPlanbookCollectionService planbookCollectionService)
		{
			_planbookCollectionService = planbookCollectionService;
		}

		[HttpGet]
		public async Task<IActionResult> GetPlanbookCollectionsByTeacherId(string teacherId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookCollectionService.GetPlanbookCollectionsByTeacherIdAsync(teacherId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetCollectionById(string collectionId)
		{
			var response = await _planbookCollectionService.GetCollectionByIdAsync(collectionId);
			return Ok(response);
		}
	}
}
