using Elepla.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
	public class ArticleController : BaseController
	{
		private readonly IArticleService _articleService;

		public ArticleController(IArticleService articleService)
		{
			_articleService = articleService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllArticleAsync(int pageIndex = 0, int pageSize = 10)
		{
			var response = await _articleService.GetAllArticleAsync(pageIndex, pageSize);
			return Ok(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetArticleByIdAsync(string id)
		{
			var response = await _articleService.GetArticleByIdAsync(id);
			return Ok(response);
		}
	}
}
