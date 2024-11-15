using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.PlanbookCollectionViewModels;
using Microsoft.AspNetCore.Authorization;
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
		[Authorize]
		public async Task<IActionResult> GetPlanbookCollectionsByTeacherId(string teacherId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookCollectionService.GetPlanbookCollectionsByTeacherIdAsync(teacherId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetCreatedPlanbookCollectionsByTeacherId(string teacherId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookCollectionService.GetCreatedPlanbookCollectionsByTeacherIdAsync(teacherId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetSavedPlanbookCollectionsByTeacherId(string teacherId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _planbookCollectionService.GetSavedPlanbookCollectionsByTeacherIdAsync(teacherId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetCollectionById(string collectionId)
		{
			var response = await _planbookCollectionService.GetCollectionByIdAsync(collectionId);
			return Ok(response);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreatePlanbookCollection(CreatePlanbookCollectionDTO model)
		{
			var response = await _planbookCollectionService.CreatePlanbookCollectionAsync(model);
			return Ok(response);
		}

		[HttpPut]
		[Authorize]
		public async Task<IActionResult> UpdatePlanbookCollection(UpdatePlanbookCollectionDTO model)
		{
			var response = await _planbookCollectionService.UpdatePlanbookCollectionAsync(model);
			return Ok(response);
		}

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> DeletePlanbookCollection(string collectionId, string teacherId)
		{
			var response = await _planbookCollectionService.DeletePlanbookCollectionAsync(collectionId, teacherId);
			return Ok(response);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> SavePlanbook(SavePlanbookDTO model)
		{
			var response = await _planbookCollectionService.SavePlanbookAsync(model);
			return Ok(response);
		}
	}
}
