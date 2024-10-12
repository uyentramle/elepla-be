using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.ChapterViewModels;

using Elepla.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
    public class ChapterController : BaseController
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChapterAsync(string? keyword, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _chapterService.GetAllChapterAsync(keyword, pageIndex, pageSize);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetChapterByIdAsync(string chapterId)
        {
            var response = await _chapterService.GetChapterByIdAsync(chapterId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapterAsync(CreateChapterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _chapterService.CreateChapterAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChapterAsync(UpdateChapterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _chapterService.UpdateChapterAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChapterAsync(string chapterId)
        {
            var response = await _chapterService.DeleteChapterAsync(chapterId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}





