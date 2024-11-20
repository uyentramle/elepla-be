using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.QuestionBankViewModels;
using Elepla.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
	public class QuestionBankController : BaseController
	{
		private readonly IQuestionBankService _questionBankService;
		public QuestionBankController(IQuestionBankService questionBankService)
		{
			_questionBankService = questionBankService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllQuestionBankAsync(string? keyword, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _questionBankService.GetAllQuestionBankAsync(keyword, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetQuestionBankByIdAsync(string id)
		{
			var response = await _questionBankService.GetQuestionBankByIdAsync(id);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetQuestionByChapterIdAsync(string chapterId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _questionBankService.GetQuestionsByChapterIdAsync(chapterId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetQuestionByLessonIdAsync(string lessonId, int pageIndex = 0, int pageSize = 10)
		{
			var response = await _questionBankService.GetQuestionsByLessonIdAsync(lessonId, pageIndex, pageSize);
			return Ok(response);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateQuestionAsync(CreateQuestionDTO model)
		{
			var response = await _questionBankService.CreateQuestionAsync(model);
			return Ok(response);
		}

		[HttpPut]
		[Authorize]
		public async Task<IActionResult> UpdateQuestionAsync(UpdateQuestionDTO model)
		{
			var response = await _questionBankService.UpdateQuestionAsync(model);
			return Ok(response);
		}

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> DeleteQuestionAsync(string id)
		{
			var response = await _questionBankService.DeleteQuestionAsync(id);
			return Ok(response);
		}
	}
}
