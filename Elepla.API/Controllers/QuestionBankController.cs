using Elepla.Service.Interfaces;
using Elepla.Service.Services;
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
		public async Task<IActionResult> GetAllQuestionBankAsync(int pageIndex = 0, int pageSize = 10)
		{
			var response = await _questionBankService.GetAllQuestionBankAsync(pageIndex, pageSize);
			return Ok(response);
		}
	}
}
